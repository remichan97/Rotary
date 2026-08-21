using System.Text.Json.Serialization;
using Rotary.Core.Http.Records;

namespace Rotary.Core.Collections.Records
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(Folder), "folder")]
    [JsonDerivedType(typeof(Request), "request")]
    public abstract record CollectionNodeDefinition
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }

        public sealed record Folder : CollectionNodeDefinition
        {
            public IList<CollectionNodeDefinition> Items { get; init; } = [];
        }

        public sealed record Request : CollectionNodeDefinition
        {
            public required RequestDefinition Definition { get; init; }
        }
    }
}
