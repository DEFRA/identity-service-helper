// <copyright file="RoleRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Roles;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Logging;

public partial class RoleRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<RoleRepository> logger) : IRoleRepository
{
    public async Task<bool> ValidateReferenceById(Guid id, CancellationToken cancellationToken = default)
    {
        LogValidatingCountyParishHoldingReferenceWithId(logger, id);

        var entity = await readOnlyContext.Roles
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

        return entity != null;
    }

    public Task<List<Roles>> GetList(Expression<Func<Roles, bool>> predicate, CancellationToken cancellationToken = default)
    {
        LogGettingAllRoles(logger);

        return readOnlyContext.Roles.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Roles?> GetSingle(Expression<Func<Roles, bool>> predicate, CancellationToken cancellationToken = default)
    {
        LogGettingSingleRole(logger);

        var result = await readOnlyContext.Roles
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return result;
    }

    public async Task<Roles> Create(Roles entity, CancellationToken cancellationToken = default)
    {
        var role = await context.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return role.Entity;
    }

    public Task<Roles> Update(Roles entity, CancellationToken cancellationToken = default)
    {
        context.Roles.Update(entity);
        return Task.FromResult(entity);
    }
}
