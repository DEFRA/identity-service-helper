// <copyright file="RoleRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Roles;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Logging;

/// <inheritdoc />
public partial class RoleRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<RoleRepository> logger) : IRoleRepository
{
    public Task<List<Roles>> GetList(
        Expression<Func<Roles, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        LogGettingAllRoles(logger);

        return readOnlyContext.Roles.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Roles?> GetSingle(
        Expression<Func<Roles, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        LogGettingSingleRole(logger);

        var result = await readOnlyContext.Roles
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return result;
    }

    public async Task<Roles> Create(Roles entity, CancellationToken cancellationToken = default)
    {
        LogCreatingRole(logger);

        var addedEntity = await context.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result =
            await readOnlyContext.Roles.SingleAsync(e => e.Id == addedEntity.Entity.Id, cancellationToken);

        return result;
    }
}
