namespace TITSWebSocket.TITSApi;

public readonly record struct TITSItemListResponse(
    long timestamp,
    RequestID requestID,
    MessageType messageType,
    TITSItemListResponseData data) : ITITSApiResponse
{
    public APIName apiName { get; init; } = APIName.TITSPublicApi;
    public Version apiVersion { get; init; } = TITSConstants.APIVersionInUse;
}

public readonly record struct TITSItemListResponseData(TITSItem[] items);

/// <summary>
/// This request lets you get a list of all available items in the users Model-Importer
/// </summary>
public readonly record struct TITSItemListRequest(
    bool sendImage, 
    RequestID requestID): ITITSApiRequest
{
    public MessageType messageType => MessageType.TITSItemListRequest;
    public APIName apiName { get; init; } = APIName.TITSPublicApi;
    public Version apiVersion { get; init; } = TITSConstants.APIVersionInUse;
}