namespace Rotary.Core.Data
{
    // Environments/EnvironmentVariables/Cookies are unused today (backlog features), but the DDL
    // lives here now so bringing those features online is a repository, not a schema migration.
    internal static class Schema
    {
        public const string CreateTablesSql = """
            CREATE TABLE IF NOT EXISTS Collections (
                Id   TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                Data TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Environments (
                Id   TEXT PRIMARY KEY,
                Name TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS EnvironmentVariables (
                EnvironmentId TEXT NOT NULL REFERENCES Environments(Id),
                Key           TEXT NOT NULL,
                Value         TEXT NOT NULL,
                PRIMARY KEY (EnvironmentId, Key)
            );

            CREATE TABLE IF NOT EXISTS Cookies (
                Id       TEXT PRIMARY KEY,
                Domain   TEXT NOT NULL,
                Path     TEXT NOT NULL,
                Name     TEXT NOT NULL,
                Value    TEXT NOT NULL,
                Expiry   TEXT NULL,
                Secure   INTEGER NOT NULL,
                HttpOnly INTEGER NOT NULL
            );

            CREATE INDEX IF NOT EXISTS IX_Cookies_Domain ON Cookies(Domain);
            """;
    }
}
