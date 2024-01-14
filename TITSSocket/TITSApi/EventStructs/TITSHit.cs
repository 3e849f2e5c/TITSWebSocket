namespace TITSWebSocket.TITSApi;

/// <summary>
/// This Event gets broadcasted whenever the player gets hit!
/// </summary>
public readonly record struct TITSHitEvent(
    long timestamp,
    RequestID requestID,
    MessageType messageType,
    TITSHitResponseData data) : ITITSApiResponse
{
    public APIName apiName { get; init; } = APIName.TITSPublicApi;
    public Version apiVersion { get; init; } = TITSConstants.APIVersionInUse;
}

public readonly record struct TITSHitResponseData(
    Guid itemId,
    string itemName,
    Guid triggerId,
    string triggerName,
    float strength,
    HitVector3 direction);

public readonly record struct HitVector3(float x, float y, float z);