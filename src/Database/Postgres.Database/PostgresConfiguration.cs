// <copyright file="PostgresConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database;

public class PostgresConfiguration
{
    public required string DefaultHost { get; init; }

    public required string ReadOnlyHost { get; init; }

    public int Port { get; init; } = 5432;

    public string Name { get; init; } = string.Empty;

    public string User { get; init; } = string.Empty;
}
