using Rotary.Core.Collections.Records;

namespace Rotary.Core.Data.Repositories
{
    internal interface ICollectionsRepository
    {
        Task<IList<CollectionIndexEntryDefinition>> GetIndexAsync();
        Task<CollectionDefinition?> GetByIdAsync(Guid id);
        Task SaveAsync(CollectionDefinition collection);
        Task DeleteAsync(Guid id);
    }
}
