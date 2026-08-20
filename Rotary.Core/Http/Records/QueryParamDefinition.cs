namespace Rotary.Core.Http.Records
{
    public record QueryParamDefinition
    {
        public string Name { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;
    }
}
