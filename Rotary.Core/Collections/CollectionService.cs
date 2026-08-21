using Rotary.Core.Collections.Records;
using Rotary.Core.Data;
using Rotary.Core.Data.Repositories;

namespace Rotary.Core.Collections
{
    // Public entry point for collection persistence. Everything SQLite/Dapper-shaped
    // (RotaryDatabase, ICollectionsRepository) is an implementation detail behind this.
    public sealed class CollectionService : ICollectionService
    {
        private readonly ICollectionsRepository _repository;

        public CollectionService(string? databasePath = null)
        {
            var database = new RotaryDatabase(databasePath);
            database.Initialize();
            _repository = new CollectionsRepository(database);
        }

        public Task<IList<CollectionIndexEntryDefinition>> GetCollectionIndexAsync() =>
            _repository.GetIndexAsync();

        public Task<CollectionDefinition?> GetCollectionAsync(Guid id) =>
            _repository.GetByIdAsync(id);

        public Task SaveCollectionAsync(CollectionDefinition collection) =>
            _repository.SaveAsync(collection);

        public Task DeleteCollectionAsync(Guid id) => _repository.DeleteAsync(id);
    }
}
