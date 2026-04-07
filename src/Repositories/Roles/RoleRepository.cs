// <copyright file="RoleRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Roles;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Logging;

public class RoleRepository(ReadOnlyPostgresDbContext readOnlyContext, ILogger<RoleRepository> logger) : IRoleRepository
{
    public async Task<bool> ValidateReferenceById(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Validating county parish holding reference with id {Id}", id);

        var entity = await readOnlyContext.Roles
            .SingleOrDefaultAsync((entity) => entity.Id == id, cancellationToken);

        return entity != null;
    }

    public async Task<Roles?> GetSingle(Expression<Func<Roles, bool>> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting single role");

        var result = await readOnlyContext.Roles
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return result;
    }
}
