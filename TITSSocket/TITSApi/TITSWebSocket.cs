using System.Collections.Immutable;
using Newtonsoft.Json;
using Websocket.Client;

namespace TITSWebSocket.TITSApi;

public class TITSWebSocket
{
    private readonly WebsocketClient apiClient;
    private readonly WebsocketClient eventsClient;

    private byte freshRequestID;

    private Dictionary<MessageType, List<Action<ITITSApiResponse>>>
        eventSubscribers = new();

    public RequestID CreateRequestID()
    {
        return new RequestID($"AnekoAPI_{Increment()}");
        
        byte Increment()
        {
            if (freshRequestID is byte.MaxValue)
            {
                freshRequestID = 0;
            }
            return freshRequestID++;
        }
    }

    public bool IsConnected
        => apiClient.IsRunning && eventsClient.IsRunning;

    private readonly List<SentTITSRequest> sentRequests = [];

    private readonly Action onConnectedEvent;
    
    public TITSWebSocket(Action onConnected)
    {
        onConnectedEvent = onConnected;
        apiClient = new WebsocketClient(TITSConstants.ApiWebSocketUri);
        eventsClient = new WebsocketClient(TITSConstants.EventsWebSocketUri);
        InitializeWebSockets();
    }

    public bool SendRequest<T>(ITITSApiRequest request, Action<T> onResponse) where T : ITITSApiResponse
    {
        if (!IsConnected)
        {
            Console.WriteLine("Unable to send request: WebSocket is not connected.");
            return false;
        }

        var sentRequest = new SentTITSRequest(request, response =>
        {
            if (response is not T typedResponse)
            {
                Console.WriteLine($"Received response of type {response.GetType().Name}, expected {typeof(T).Name}.");
                return;
            }

            onResponse(typedResponse);
        });
        sentRequests.Add(sentRequest);

        var json = JsonConvert.SerializeObject(request);
        apiClient.Send(json);
        return true;
    }
    
    public void SubscribeToEvent<T>(Action<T> onEvent) where T : ITITSApiResponse
    {
        if (typeof(T) == typeof(TITSHitEvent))
        {
            AddSubscriber(MessageType.TITSHitEvent, r => onEvent((T)r));
        }
        else if (typeof(T) == typeof(TITSTriggerActivatedEvent))
        {
            AddSubscriber(MessageType.TITSTriggerActivatedEvent, r => onEvent((T)r));
        }
        else if (typeof(T) == typeof(TITSTriggerEndedEvent))
        {
            AddSubscriber(MessageType.TITSTriggerEndedEvent, r => onEvent((T)r));
        }
        else
        {
            Console.WriteLine($"Unable to subscribe to event of type {typeof(T).Name}: Unknown event type.");
        }

        return;
        void AddSubscriber(MessageType type, Action<ITITSApiResponse> subscriber)
        {
            if (eventSubscribers.TryGetValue(type, out var subscribers))
            {
                subscribers.Add(subscriber);
                return;
            }

            eventSubscribers.Add(type, [subscriber]);
        }
    }

    public void Update()
    {
        foreach (var sentRequest in sentRequests)
        {
            sentRequest.CheckTimeout();
        }

        sentRequests.RemoveAll(request => request.IsTimedOut);
    }

    private async void InitializeWebSockets()
    {
        for (int attempts = 0; attempts < 5; attempts++)
        {
            try
            {
                await apiClient.StartOrFail();
                await eventsClient.StartOrFail();
                
                SubscribeEvents();
                onConnectedEvent();
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to connect to WebSocket server. Retrying in 5 seconds. (Attempt {attempts + 1}/5)\n{e}");
                Thread.Sleep(5000);
                attempts++;
            }
        }
    }

    private void SubscribeEvents()
    {
        if (!IsConnected)
        {
            Console.WriteLine("Unable to subscribe to events: WebSocket is not connected.");
            return;
        }
        
        apiClient.MessageReceived.Subscribe(msg =>
        {
            if (msg.Text is not { } text) { return; }
            // FIXME this blows it crashes on APIError
            ITITSApiResponse response = JsonConvert.DeserializeObject<TITSApiResponse>(text);

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (response.messageType)
            {
                case MessageType.TITSItemListResponse:
                    response = JsonConvert.DeserializeObject<TITSItemListResponse>(text);
                    break;
                case MessageType.TITSThrowItemsResponse:
                    response = JsonConvert.DeserializeObject<TITSThrowItemsResponse>(text);
                    break;
                case MessageType.TITSItemInfoResponse:
                    response = JsonConvert.DeserializeObject<TITSItemInfoResponse>(text);
                    break;
                case MessageType.TITSTriggerListResponse:
                    response = JsonConvert.DeserializeObject<TITSTriggerListResponse>(text);
                    break;
                case MessageType.TITSTriggerActivateResponse:
                    response = JsonConvert.DeserializeObject<TITSActivateTriggerResponse>(text);
                    break;
                default:
                    Console.WriteLine($"Received response of unknown type {response.messageType}.");
                    break;
            }

            foreach (var request in sentRequests.ToImmutableArray())
            {
                if (!request.IsForThisRequest(response)) { continue; }
                request.OnResponseReceived(response);
                sentRequests.Remove(request);
                return;
            }
        });

        eventsClient.MessageReceived.Subscribe(msg =>
        {
            if (msg.Text is not { } text) { return; }

            // FIXME do this better
            ITITSApiResponse response = JsonConvert.DeserializeObject<TITSApiResponse>(text);

            switch (response.messageType)
            {
                case MessageType.TITSHitEvent:
                    var hitEvent = JsonConvert.DeserializeObject<TITSHitEvent>(text);
                    TriggerEvents(MessageType.TITSHitEvent, hitEvent);
                    break;
                case MessageType.TITSTriggerActivatedEvent:
                    var triggerActivatedEvent = JsonConvert.DeserializeObject<TITSTriggerActivatedEvent>(text);
                    TriggerEvents(MessageType.TITSTriggerActivatedEvent, triggerActivatedEvent);
                    break;
                case MessageType.TITSTriggerEndedEvent:
                    var triggerEndedEvent = JsonConvert.DeserializeObject<TITSTriggerEndedEvent>(text);
                    TriggerEvents(MessageType.TITSTriggerEndedEvent, triggerEndedEvent);
                    break;
                default:
                    Console.WriteLine($"Received response of unknown type {response.messageType}.");
                    break;
            }
            
            void TriggerEvents<T>(MessageType type, T resp) where T : ITITSApiResponse
            {
                if (!eventSubscribers.TryGetValue(type, out var subscribers)) { return; }
                foreach (var action in subscribers) { action(resp); }
            }
        });
    }
}