namespace TITSWebSocket.TITSApi;

public class SentTITSRequest(ITITSApiRequest request, Action<ITITSApiResponse> onResponse)
{
    private readonly DateTimeOffset timeOut = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5);
    public bool IsTimedOut;
    
    public void CheckTimeout()
    {
        if (DateTimeOffset.UtcNow < timeOut) { return; }

        IsTimedOut = true;
        Console.WriteLine($"Request {request.requestID.Value} timed out.");
    }

    public bool IsForThisRequest(ITITSApiResponse response) 
        => request.requestID.Equals(response.requestID) &&
           request.messageType.IsMatchingResponse(response.messageType);

    public void OnResponseReceived(ITITSApiResponse response)
        => onResponse(response);
}