using System.Net.Http.Headers;

namespace Rotary.Core.Http
{
    public record RequestDefinition
    {
        public HttpMethod Method { get; init; } = HttpMethod.Get;
        public string Url { get; init; } = string.Empty;
        public IList<KeyValuePair<string, string>> Headers { get; init; } = [];
        public string ContentType { get; init; } = "application/json";
        public object? Body { get; init; } = null;
    }
}
