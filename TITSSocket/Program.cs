using System.Collections.Immutable;
using TITSWebSocket.TITSApi;

namespace TITSWebSocket;

public static class Program
{
    private static TITSApi.TITSWebSocket webSocket = null!;
    
    private static ImmutableHashSet<TITSItem> items = ImmutableHashSet<TITSItem>.Empty;
    private static ImmutableHashSet<TITSTrigger> triggers = ImmutableHashSet<TITSTrigger>.Empty;

    public static void Main(string[] args)
    {
        webSocket = new TITSApi.TITSWebSocket(onConnected: Init);
        while (true)
        {
            webSocket.Update();
            Thread.Sleep(100);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private static void Init()
    {
        var itemRequest = new TITSItemListRequest(sendImage: false, webSocket.CreateRequestID());
        webSocket.SendRequest<TITSItemListResponse>(itemRequest, itemResp =>
        {
            Console.WriteLine("Received item list");
            items = itemResp.data.items.ToImmutableHashSet();
            
            var triggerRequest = new TITSTriggerListRequest(webSocket.CreateRequestID());
            webSocket.SendRequest<TITSTriggerListResponse>(triggerRequest, triggerResp =>
            {
                Console.WriteLine("Received trigger list");
                triggers = triggerResp.data.triggers.ToImmutableHashSet();
                OnItemsAndTriggersReceived();
            });
        });
    }

    private static void OnItemsAndTriggersReceived()
    {
        var throwData = new TITSThrowItemsRequestData(
            itemsToThrow: items,
            delayTime: 0,
            amountOfThrows: 1,
            errorOnMissingID: false);

        var throwRequest = new TITSThrowItemsRequest(throwData, webSocket.CreateRequestID());

        webSocket.SendRequest<TITSThrowItemsResponse>(throwRequest, response =>
        {
            Console.WriteLine("Received throw response");
            Console.WriteLine($"threw {response.data.numberOfThrownItems} items");
        });
        
        var triggerData = new TITSActivateTriggerRequestData(triggers.First());
        var triggerRequest = new TITSActivateTriggerRequest(triggerData, webSocket.CreateRequestID());
        
        webSocket.SendRequest<TITSActivateTriggerResponse>(triggerRequest, _ =>
        {
            Console.WriteLine("Received trigger response");
        });
    }
}