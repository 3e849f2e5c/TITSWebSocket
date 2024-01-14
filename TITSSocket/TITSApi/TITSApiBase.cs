using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TITSWebSocket.Utils;

namespace TITSWebSocket.TITSApi;

[JsonConverter(typeof(RequestIdJsonConverter))]
public readonly record struct RequestID(string Value)
{
    public bool Equals(RequestID other)
        => Value == other.Value;

    public override int GetHashCode()
        => Value.GetHashCode();
}

[JsonConverter(typeof(StringEnumConverter))]
public enum MessageType
{
    TITSItemListRequest,
    TITSItemListResponse,
    TITSThrowItemsRequest,
    TITSThrowItemsResponse,
    TITSItemInfoRequest,
    TITSItemInfoResponse,
    TITSTriggerListRequest,
    TITSTriggerListResponse,
    TITSTriggerActivateRequest,
    TITSTriggerActivateResponse,
    TITSHitEvent,
    TITSTriggerActivatedEvent,
    TITSTriggerEndedEvent
}

public static class MessageTypeExtension
{
    public static bool IsMatchingResponse(this MessageType messageType, MessageType other)
        => messageType switch
        {
            MessageType.TITSItemListRequest => other == MessageType.TITSItemListResponse,
            MessageType.TITSThrowItemsRequest => other == MessageType.TITSThrowItemsResponse,
            MessageType.TITSItemInfoRequest => other == MessageType.TITSItemInfoResponse,
            MessageType.TITSTriggerListRequest => other == MessageType.TITSTriggerListResponse,
            MessageType.TITSTriggerActivateRequest => other == MessageType.TITSTriggerActivateResponse,
            _ => false
        };
}

public interface ITITSApiRequest
{
    public APIName apiName { get; init; }
    public Version apiVersion { get; init; }
    public RequestID requestID { get; init; }
    public MessageType messageType { get; }
}

public interface ITITSApiResponse
{
    public APIName apiName { get; init; }
    public Version apiVersion { get; init; }
    public long timestamp { get; init; }
    public RequestID requestID { get; init; }
    public MessageType messageType { get; init; }
}

public readonly record struct TITSItem(Guid ID, string name, string encodedImage);
public readonly record struct TITSTrigger(Guid ID, string name);

public readonly record struct TITSApiResponse(
    long timestamp,
    RequestID requestID,
    MessageType messageType,
    APIName apiName,
    Version apiVersion
) : ITITSApiResponse;