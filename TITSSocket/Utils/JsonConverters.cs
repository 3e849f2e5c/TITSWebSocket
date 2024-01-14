using Newtonsoft.Json;
using TITSWebSocket.TITSApi;

namespace TITSWebSocket.Utils;

public class RequestIdJsonConverter : JsonConverter<RequestID>
{
    public override void WriteJson(JsonWriter writer, RequestID value, JsonSerializer serializer)
        => writer.WriteValue(value.Value);

    public override RequestID ReadJson(JsonReader reader, Type objectType, RequestID existingValue, bool hasExistingValue, JsonSerializer serializer)
        => new(reader.Value as string ?? throw new JsonReaderException("Expected string"));
}