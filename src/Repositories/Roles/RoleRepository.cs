// <copyright file="RoleRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Roles;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Logging;

public class RoleRepository(PostgresDbContext context, ILogger<RoleRepository> logger) : IRoleRepository
{
    public async Task<Roles?> GetSingle(Expression<Func<Roles, bool>> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting single role");

        var result = await context.Roles
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return result;
    }
}
