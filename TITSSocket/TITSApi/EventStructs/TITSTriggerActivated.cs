namespace TITSWebSocket.TITSApi;

/// <summary>
/// This event gets broadcasted whenever a trigger threw its first item!
/// </summary>
public readonly record struct TITSTriggerActivatedEvent(
    long timestamp,
    RequestID requestID,
    MessageType messageType,
    TITSTriggerActivatedData data) : ITITSApiResponse
{
    public APIName apiName { get; init; } = APIName.TITSPublicApi;
    public Version apiVersion { get; init; } = TITSConstants.APIVersionInUse;
}

public readonly record struct TITSTriggerActivatedData(Guid triggerID);