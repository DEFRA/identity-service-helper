// <copyright file="RoleRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Roles;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Logging;

public class RoleRepository(PostgresDbContext context, ILogger<RoleRepository> logger) : IRoleRepository, IRepository<Roles>
{
    public async Task<Roles?> GetSingle(Expression<Func<Roles, bool>> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting single role");

        var result = await context.Roles
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

    public async Task Delete(Roles entity, CancellationToken cancellationToken = default)
    {
        context.Roles.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<IEnumerable<Roles>> List(Expression<Func<Roles, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Roles>>(context.Roles.Where(predicate).AsEnumerable());
    }

    public Task<List<Roles>> GetList(Expression<Func<Roles, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return context.Roles.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<bool> Delete(Expression<Func<Roles, bool>> predicate, Guid operatorId, CancellationToken cancellationToken = default)
    {
        var entities = await context.Roles.Where(predicate).ToListAsync(cancellationToken);
        if (entities.Count == 0)
        {
            return false;
        }

        context.Roles.RemoveRange(entities);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
