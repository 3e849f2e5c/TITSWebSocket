namespace TITSWebSocket.TITSApi;

public readonly record struct TITSActivateTriggerResponse(
    long timestamp,
    APIName apiName,
    Version apiVersion,
    RequestID requestID,
    MessageType messageType) : ITITSApiResponse;

public readonly struct TITSActivateTriggerRequestData(TITSTrigger trigger)
{
    public readonly Guid triggerId = trigger.ID;
}

public readonly record struct TITSActivateTriggerRequest(
    TITSActivateTriggerRequestData data,
    RequestID requestID) : ITITSApiRequest
{
    public MessageType messageType => MessageType.TITSTriggerActivateRequest;
    public APIName apiName { get; init; } = APIName.TITSPublicApi;
    public Version apiVersion { get; init; } = TITSConstants.APIVersionInUse;
}