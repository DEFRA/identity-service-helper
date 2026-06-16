// <copyright file="CphDelegationsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Logging;

public partial class CphDelegationsRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<CphDelegationsRepository> logger)
    : ICphDelegationsRepository
{
    public async Task<CountyParishHoldingDelegations?> GetSingle(
        Expression<Func<CountyParishHoldingDelegations, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        LogGettingSingleDelegation();

        var result = await readOnlyContext.CountyParishHoldingDelegations
            .Include(p => p.CountyParishHolding)
            .ThenInclude(p => p.ApplicationUserAccountHoldingAssignments)
            .ThenInclude(p => p.UserAccount)
            .Include(p => p.DelegatingUser)
            .Include(p => p.DelegatedUser)
            .Include(p => p.DelegatedUserRole)
            .SingleOrDefaultAsync(predicate, cancellationToken);

        return result;
    }

    public async Task<List<CountyParishHoldingDelegations>> GetList(
        Expression<Func<CountyParishHoldingDelegations, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        LogGettingListOfCountyParishHoldingDelegations();

        var results = await readOnlyContext.CountyParishHoldingDelegations
            .Include(p => p.CountyParishHolding)
            .ThenInclude(p => p.ApplicationUserAccountHoldingAssignments)
            .ThenInclude(p => p.UserAccount)
            .Include(p => p.DelegatingUser)
            .Include(p => p.DelegatedUser)
            .Include(p => p.DelegatedUserRole)
            .Where(predicate)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return results;
    }

    public async Task<CountyParishHoldingDelegations> Create(
        CountyParishHoldingDelegations entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        LogCreatingDelegationWithId(entity.Id);

        var addedEntry = await context.CountyParishHoldingDelegations.AddAsync(entity, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        var result = await readOnlyContext.CountyParishHoldingDelegations
            .Include(p => p.CountyParishHolding)
            .Include(p => p.DelegatingUser)
            .Include(p => p.DelegatedUser)
            .Include(p => p.DelegatedUserRole)
            .SingleAsync(c => c.Id == addedEntry.Entity.Id, cancellationToken);

        return result;
    }

    public async Task<CountyParishHoldingDelegations> Update(
        CountyParishHoldingDelegations entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        LogUpdatingDelegationWithId(entity.Id);

        context.Update(entity);

        await context.SaveChangesAsync(cancellationToken);

        var result = await readOnlyContext.CountyParishHoldingDelegations
            .Include(p => p.CountyParishHolding)
            .Include(p => p.DelegatingUser)
            .Include(p => p.DelegatedUser)
            .Include(p => p.DelegatedUserRole)
            .SingleAsync(c => c.Id == entity.Id, cancellationToken);

        return result;
    }
}
