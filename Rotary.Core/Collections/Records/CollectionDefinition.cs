namespace Rotary.Core.Collections.Records
{
    public sealed record CollectionDefinition
    {
        public required Guid Id { get; init; } = Guid.NewGuid();
        public required string Name { get; init; }
        public IList<CollectionNodeDefinition> Items { get; init; } = [];
    }
}
