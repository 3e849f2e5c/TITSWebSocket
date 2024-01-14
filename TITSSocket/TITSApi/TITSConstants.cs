using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TITSWebSocket.TITSApi;

[JsonConverter(typeof(StringEnumConverter))]
public enum APIName
{
    TITSPublicApi
}

public static class TITSConstants
{
    public static readonly Version APIVersionInUse = new(major: 1, minor: 0);
    public static readonly Uri ApiWebSocketUri = new("ws://localhost:42069/websocket"); // FIXME make port configurable
    public static readonly Uri EventsWebSocketUri = new("ws://localhost:42069/events"); // FIXME make port configurable
}