// <copyright file="IPostgresIamTokenGeneratorService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database;

public interface IPostgresIamTokenGeneratorService
{
    Task<string> GenerateAuthTokenAsync(string hostname, int port, string username);
}
