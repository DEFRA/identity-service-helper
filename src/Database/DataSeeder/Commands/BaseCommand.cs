// <copyright file="BaseCommand.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.DataSeeder.Commands;

using System.CommandLine;
using Npgsql;

/// <summary>
/// Base class for all commands.
/// </summary>
/// <param name="name">Name of the command.</param>
/// <param name="description">Description of the command.</param>
public class BaseCommand(string name, string description)
    : Command(name, description)
{
    /// <summary>
    /// Gets the database url option.
    /// </summary>
    protected Option<string> DatabaseUrlOption { get; } = new("-db")
    {
        Description = "The url to the database.",
        Required = true,
    };

    /// <summary>
    /// Gets the database username option.
    /// </summary>
    protected Option<string> DatabaseUserNameOption { get; } = new("-uid")
    {
        Description = "The username to use to connect to the database.",
        Required = true,
    };

    /// <summary>
    /// Gets the database user password option.
    /// </summary>
    protected Option<string> DatabasePasswordOption { get; } = new("-pwd")
    {
        Description = "The password of the user.",
        Required = true,
    };

    protected static async Task<int> ExecuteNonQueryAsync(
        string connectionString,
        string commandText,
        CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(commandText, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);

        return 0;
    }

    protected static string BuildConnectionString(
        string? databaseUrl,
        string? databaseUserName,
        string? databasePassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseUserName);
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePassword);

        if (databaseUrl.Contains('='))
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(databaseUrl)
            {
                Username = databaseUserName,
                Password = databasePassword,
            };

            return connectionStringBuilder.ConnectionString;
        }

        if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var databaseUri))
        {
            throw new ArgumentException("The database url must be a valid absolute URI or a PostgreSQL connection string.");
        }

        var databaseName = databaseUri.AbsolutePath.Trim('/');
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException("The database url must include a database name in the path.");
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.IsDefaultPort ? 5432 : databaseUri.Port,
            Database = databaseName,
            Username = databaseUserName,
            Password = databasePassword,
        };

        return builder.ConnectionString;
    }
}
