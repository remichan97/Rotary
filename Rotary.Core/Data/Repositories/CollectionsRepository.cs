using System.Text.Json;
using Dapper;
using Rotary.Core.Collections.Records;

namespace Rotary.Core.Data.Repositories
{
    internal sealed class CollectionsRepository : ICollectionsRepository
    {
        private readonly RotaryDatabase _database;

        public CollectionsRepository(RotaryDatabase database)
        {
            _database = database;
        }

        public async Task<IList<CollectionIndexEntryDefinition>> GetIndexAsync()
        {
            using var connection = _database.CreateConnection();
            var rows = await connection.QueryAsync<CollectionIndexRow>(
                "SELECT Id, Name FROM Collections ORDER BY Name;"
            );
            return rows.Select(row => new CollectionIndexEntryDefinition
                {
                    Id = Guid.Parse(row.Id),
                    Name = row.Name,
                })
                .ToList();
        }

        public async Task<CollectionDefinition?> GetByIdAsync(Guid id)
        {
            using var connection = _database.CreateConnection();
            var row = await connection.QuerySingleOrDefaultAsync<CollectionRow>(
                "SELECT Id, Name, Data FROM Collections WHERE Id = @Id;",
                new { Id = id.ToString() }
            );

            if (row is null)
            {
                return null;
            }

            var items =
                JsonSerializer.Deserialize(
                    row.Data,
                    RotaryJsonContext.Default.IListCollectionNodeDefinition
                ) ?? [];

            return new CollectionDefinition
            {
                Id = Guid.Parse(row.Id),
                Name = row.Name,
                Items = items,
            };
        }

        public async Task SaveAsync(CollectionDefinition collection)
        {
            string data = JsonSerializer.Serialize(
                collection.Items,
                RotaryJsonContext.Default.IListCollectionNodeDefinition
            );

            using var connection = _database.CreateConnection();
            await connection.ExecuteAsync(
                """
                INSERT INTO Collections (Id, Name, Data)
                VALUES (@Id, @Name, @Data)
                ON CONFLICT(Id) DO UPDATE SET Name = excluded.Name, Data = excluded.Data;
                """,
                new
                {
                    Id = collection.Id.ToString(),
                    Name = collection.Name,
                    Data = data,
                }
            );
        }

        public async Task DeleteAsync(Guid id)
        {
            using var connection = _database.CreateConnection();
            await connection.ExecuteAsync(
                "DELETE FROM Collections WHERE Id = @Id;",
                new { Id = id.ToString() }
            );
        }

        private sealed class CollectionIndexRow
        {
            public string Id { get; init; } = string.Empty;
            public string Name { get; init; } = string.Empty;
        }

        private sealed class CollectionRow
        {
            public string Id { get; init; } = string.Empty;
            public string Name { get; init; } = string.Empty;
            public string Data { get; init; } = string.Empty;
        }
    }
}
