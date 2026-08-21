namespace Rotary.Core.Collections.Records
{
    public sealed record CollectionIndexEntryDefinition
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
    }
}
