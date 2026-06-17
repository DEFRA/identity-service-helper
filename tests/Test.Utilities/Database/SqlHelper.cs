// <copyright file="SqlHelper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Test.Utilities.Database;

using System.Diagnostics.CodeAnalysis;
using Npgsql;

[ExcludeFromCodeCoverage]
public static class SqlHelper
{
    public static async Task ExecuteSqlFile(string connectionString, string sqlFilePath)
    {
        var sql = await File.ReadAllTextAsync(sqlFilePath);

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // Split by semicolon, but be careful with DO blocks or other constructs that might contain semicolons
        // For the current testcontainer script, splitting by ; and filtering empty entries should work
        // but it's better to handle DO blocks or use a more robust parser if available.
        // Given the known structure of testcontainer.postgresql.sql (mostly DO block or single statements):
        var trimmedSql = sql.Trim();
        if (trimmedSql.StartsWith("DO", StringComparison.OrdinalIgnoreCase) || trimmedSql.StartsWith("--", StringComparison.OrdinalIgnoreCase))
        {
            await using var cmd = new NpgsqlCommand(sql, conn);
            await cmd.ExecuteNonQueryAsync();
        }
        else
        {
            var statements = sql.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var statement in statements)
            {
                if (string.IsNullOrWhiteSpace(statement))
                {
                    continue;
                }

                await using var cmd = new NpgsqlCommand(statement, conn);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
