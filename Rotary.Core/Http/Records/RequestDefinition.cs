namespace Rotary.Core.Http.Records
{
    public record RequestDefinition
    {
        public HttpMethod Method { get; init; } = HttpMethod.Get;
        public string Url { get; init; } = string.Empty;
        public IList<KeyValuePair<string, string>> Headers { get; init; } = [];

        // Authentication scheme to use for the request, if any. If not provided, no authentication will be used.
        public AuthDefinition Auth { get; init; } = new AuthDefinition.NoAuthDefinition();

        // List of query parameters, each represented as a name-value pair, UI can provide a way to temporary exclude a query parameter from the request
        // But that's not on us here. We only process what is provided to us.
        public IList<QueryParamDefinition> QueryParams { get; init; } = [];
        public string ContentType { get; init; } = "application/json";
        public object? Body { get; init; } = null;
    }
}
