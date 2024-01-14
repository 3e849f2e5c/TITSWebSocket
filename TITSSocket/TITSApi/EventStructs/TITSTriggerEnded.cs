namespace TITSWebSocket.TITSApi;

/// <summary>
/// This event gets broadcasted whenever a trigger threw its last item!
/// </summary>
public readonly record struct TITSTriggerEndedEvent(
    long timestamp,
    RequestID requestID,
    MessageType messageType,
    TITSTriggerEndedData data) : ITITSApiResponse
{
    public APIName apiName { get; init; } = APIName.TITSPublicApi;
    public Version apiVersion { get; init; } = TITSConstants.APIVersionInUse;
}

public readonly record struct TITSTriggerEndedData(Guid triggerID);