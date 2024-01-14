namespace TITSWebSocket.TITSApi;

public readonly record struct TITSThrowItemsResponse(
    long timestamp,
    APIName apiName,
    Version apiVersion,
    RequestID requestID,
    MessageType messageType,
    TITSThrowItemsResponseData data) : ITITSApiResponse;

public readonly record struct TITSThrowItemsResponseData(int numberOfThrownItems);

public readonly struct TITSThrowItemsRequestData(
    IReadOnlyCollection<TITSItem> itemsToThrow,
    float delayTime,
    int amountOfThrows,
    bool errorOnMissingID)
{
    public readonly Guid[] items = itemsToThrow.Select(item => item.ID).ToArray();
    public readonly float delayTime = delayTime;
    public readonly int amountOfThrows = amountOfThrows;
    public readonly bool errorOnMissingID = errorOnMissingID;
}

/// <summary>
/// This request lets you control the throws of T.I.T.S.
/// </summary>
/// <param name="data"></param>
/// <param name="requestID"></param>
public readonly record struct TITSThrowItemsRequest(
    TITSThrowItemsRequestData data,
    RequestID requestID): ITITSApiRequest
{
    public MessageType messageType => MessageType.TITSThrowItemsRequest;
    public APIName apiName { get; init; } = APIName.TITSPublicApi;
    public Version apiVersion { get; init; } = TITSConstants.APIVersionInUse;
}