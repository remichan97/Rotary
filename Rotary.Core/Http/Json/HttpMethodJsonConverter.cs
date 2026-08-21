using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rotary.Core.Http.Json
{
    // HttpMethod has no default STJ support (it's not a simple value type STJ knows how to
    // read/write), so RequestDefinition.Method needs this to round-trip through persistence.
    public sealed class HttpMethodJsonConverter : JsonConverter<HttpMethod>
    {
        public override HttpMethod Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        ) => new(reader.GetString() ?? HttpMethod.Get.Method);

        public override void Write(
            Utf8JsonWriter writer,
            HttpMethod value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.Method);
    }
}
