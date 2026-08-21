using System.Text.Json.Serialization;
using Rotary.Core.Collections.Records;

namespace Rotary.Core.Data
{
    // Source-generated (not reflection-based) so persistence serialization stays NativeAOT/trim-safe.
    [JsonSerializable(typeof(IList<CollectionNodeDefinition>))]
    [JsonSerializable(typeof(CollectionNodeDefinition.Folder))]
    [JsonSerializable(typeof(CollectionNodeDefinition.Request))]
    public partial class RotaryJsonContext : JsonSerializerContext { }
}
