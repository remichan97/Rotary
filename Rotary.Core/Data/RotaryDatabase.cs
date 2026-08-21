using Microsoft.Data.Sqlite;

namespace Rotary.Core.Data
{
    // One centralized SQLite file instead of scattered per-collection JSON files, so there's a
    // single, well-known thing to protect/back up rather than many loosely-related ones.
    internal sealed class RotaryDatabase
    {
        private readonly string _connectionString;

        public RotaryDatabase(string? databasePath = null)
        {
            var path = databasePath ?? GetDefaultDatabasePath();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            _connectionString = new SqliteConnectionStringBuilder { DataSource = path }.ToString();
        }

        public static string GetDefaultDatabasePath() =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Rotary",
                "rotary.db"
            );

        public SqliteConnection CreateConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public void Initialize()
        {
            using var connection = CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = Schema.CreateTablesSql;
            command.ExecuteNonQuery();
        }
    }
}
