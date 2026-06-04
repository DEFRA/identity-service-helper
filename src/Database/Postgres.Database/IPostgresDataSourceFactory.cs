// <copyright file="IPostgresDataSourceFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database;

using Npgsql;

public interface IPostgresDataSourceFactory
{
    NpgsqlDataSource CreateDataSource(string connectionIdentifier);
}
