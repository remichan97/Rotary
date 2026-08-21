using Rotary.Core.Collections.Records;

namespace Rotary.Core.Collections
{
    public interface ICollectionService
    {
        // Cheap listing for startup/sidebar — Id/Name only, no request tree.
        Task<IList<CollectionIndexEntryDefinition>> GetCollectionIndexAsync();

        // Full request tree for one collection, loaded lazily on expand/open.
        Task<CollectionDefinition?> GetCollectionAsync(Guid id);

        Task SaveCollectionAsync(CollectionDefinition collection);

        Task DeleteCollectionAsync(Guid id);
    }
}
