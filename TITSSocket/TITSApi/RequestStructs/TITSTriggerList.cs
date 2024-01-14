namespace TITSWebSocket.TITSApi;

public readonly record struct TITSTriggerListResponse(
    long timestamp,
    RequestID requestID,
    MessageType messageType,
    int triggerCount,
    TITSTriggerListResponseData data) : ITITSApiResponse
{
    public APIName apiName { get; init; } = APIName.TITSPublicApi;
    public Version apiVersion { get; init; } = TITSConstants.APIVersionInUse;
}

public readonly record struct TITSTriggerListResponseData(TITSTrigger[] triggers);

/// <summary>
/// This request will send oyu a list of all available triggers in the users T.I.T.S.-Setup.
/// </summary>
public readonly record struct TITSTriggerListRequest(
    RequestID requestID): ITITSApiRequest
{
    public MessageType messageType => MessageType.TITSTriggerListRequest;
    public APIName apiName { get; init; } = APIName.TITSPublicApi;
    public Version apiVersion { get; init; } = TITSConstants.APIVersionInUse;
}