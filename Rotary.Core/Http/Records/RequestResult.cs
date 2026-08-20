using System.Net;

namespace Rotary.Core.Http.Records
{
    public abstract record RequestResult
    {
        public sealed record Completed : RequestResult
        {
            public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;
            public required IList<KeyValuePair<string, string>> Headers { get; init; }
            public TimeSpan Duration { get; init; } = TimeSpan.Zero;
            public object? Body { get; init; } = null;
        }

        public sealed record Failed : RequestResult
        {
            public string Reason { get; init; } = string.Empty;
            public TimeSpan Duration { get; init; } = TimeSpan.Zero;
        }
    }
}
