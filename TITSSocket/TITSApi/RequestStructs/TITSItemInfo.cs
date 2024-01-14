namespace TITSWebSocket.TITSApi;

public readonly record struct TITSItemInfoResponse(
    long timestamp,
    APIName apiName,
    Version apiVersion,
    RequestID requestID,
    MessageType messageType,
    TITSItemInfoResponseData data) : ITITSApiResponse;

public readonly record struct TITSItemInfoResponseData(Guid ID, string name, string encodedImage);

public readonly struct TITSItemInfoRequestData(TITSItem trigger)
{
    public readonly Guid itemID = trigger.ID;
}

/// <summary>
/// This request will give you info about only a single item.
/// </summary>
public readonly record struct TITSItemInfoRequest(
    TITSItemInfoRequestData data,
    RequestID requestID) : ITITSApiRequest
{
    public MessageType messageType => MessageType.TITSItemInfoRequest;
    public APIName apiName { get; init; } = APIName.TITSPublicApi;
    public Version apiVersion { get; init; } = TITSConstants.APIVersionInUse;
}