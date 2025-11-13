namespace Livestock.Auth.Database;

public static class PostgreExtensions
{
    public const string UuidGenerator = "uuid-ossp";
    public const string UuidAlgorithm = "uuid_generate_v4()";
    public const string PostGis = "postgis";
    public const string PgCrypto = "pgcrypto";
    public const string PgAudit = "pgaudit";
    public const string PgTerm = "pg_term";
    public const string FuzzyStrMatch = "fuzzystrmatch";
    public const string Citext = "citext";

}