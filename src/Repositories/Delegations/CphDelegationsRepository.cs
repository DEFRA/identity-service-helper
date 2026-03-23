// <copyright file="CphDelegationsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Exceptions;
using Microsoft.Extensions.Logging;

public class CphDelegationsRepository(PostgresDbContext context, ReadOnlyPostgresDbContext readOnlyContext, ILogger<CphDelegationsRepository> logger) : ICphDelegationsRepository
{
    public async Task<CountyParishHoldingDelegations?> GetSingle(Expression<Func<CountyParishHoldingDelegations, bool>> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting single delegation");

        var result = await readOnlyContext.CountyParishHoldingDelegations
            .Include(p => p.CountyParishHolding)
            .Include(p => p.DelegatingUser)
            .Include(p => p.DelegatedUser)
            .Include(p => p.DelegatedUserRole)
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return result;
    }

    public async Task<List<CountyParishHoldingDelegations>> GetList(Expression<Func<CountyParishHoldingDelegations, bool>> predicate, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting list of CountyParishHoldingDelegations");
        
        var results = await readOnlyContext.CountyParishHoldingDelegations
            .Include(p => p.CountyParishHolding)
            .Include(p => p.DelegatingUser)
            .Include(p => p.DelegatedUser)
            .Include(p => p.DelegatedUserRole)
            .Where(predicate).ToListAsync(cancellationToken);

        return results;
    }

    public async Task<CountyParishHoldingDelegations> Create(CountyParishHoldingDelegations entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        logger.LogInformation("Creating delegation with id {Id}", entity.Id);
        var addedEntry = await context.CountyParishHoldingDelegations.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return addedEntry.Entity;
    }

    public async Task<CountyParishHoldingDelegations> Update(CountyParishHoldingDelegations entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        logger.LogInformation("Updating delegation with id {Id}", entity.Id);
        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> Delete(Expression<Func<CountyParishHoldingDelegations, bool>> predicate, Guid operatorId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting delegation with operator id {OperatorId}", operatorId);

        var delegation = await context.CountyParishHoldingDelegations
            .SingleOrDefaultAsync(predicate, cancellationToken);

        if (delegation == null)
        {
            logger.LogWarning("Delegation not found for deletion");
            throw new NotFoundException("Delegation not found");
        }

        delegation.DeletedById = operatorId;
        delegation.DeletedAt = DateTime.UtcNow;
        context.CountyParishHoldingDelegations.Update(delegation);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
