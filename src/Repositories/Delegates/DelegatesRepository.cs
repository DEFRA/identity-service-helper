// <copyright file="DelegatesRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Delegates;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Exceptions;
using Microsoft.Extensions.Logging;

public class DelegatesRepository(PostgresDbContext context, ILogger<DelegatesRepository> logger) : IDelegatesRepository
{
    public async Task<Delegations?> GetSingle(Expression<Func<Delegations, bool>> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting single delegation");
        var query = await context.Delegations
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return query;
    }

    public async Task<List<Delegations>> GetList(Expression<Func<Delegations, bool>> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting list of delegations");
        var query = await context.Delegations
            .Where(predicate).ToListAsync<Delegations>(cancellationToken);

        return query;
    }

    public async Task<Delegations> Create(Delegations entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        logger.LogInformation("Creating delegation with id {Id}", entity.Id);
        var addedEntry = await context.Delegations.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return addedEntry.Entity;
    }

    public async Task<Delegations> Update(Delegations entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        logger.LogInformation("Updating delegation with id {Id}", entity.Id);
        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> Delete(Expression<Func<Delegations, bool>> predicate, Guid operatorId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting delegation with operator id {OperatorId}", operatorId);
        var delegation = await context.Delegations
            .SingleOrDefaultAsync(predicate, cancellationToken);

        if (delegation == null)
        {
            logger.LogWarning("Delegation not found for deletion");
            throw new NotFoundException("Delegation not found");
        }

        delegation.DeletedById = operatorId;
        delegation.DeletedAt = DateTime.UtcNow;
        context.Delegations.Update(delegation);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
