// <copyright file="CphDelegationsForDelegatorRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using Defra.Identity.Repositories.Common.Exceptions;
using Microsoft.Extensions.Logging;

public class CphDelegationsForDelegatorRepository(
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<CphDelegationsForDelegateRepository> logger) : ICphDelegationsForDelegatorRepository
{
    private Expression<Func<ApplicationUserAccountHoldingAssignments, bool>>? HoldingAssignmentsFilter { get; set; }

    public ICphDelegationsForDelegatorRepository WithHoldingAssignmentsFilter(Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> holdingAssignmentsFilter)
    {
        HoldingAssignmentsFilter = holdingAssignmentsFilter;
        return this;
    }

    public async Task<PagedEntities<CountyParishHoldingDelegations>> GetPaged<TOrderBy>(
        Expression<Func<UserAccounts, bool>> primaryPredicate,
        Expression<Func<CountyParishHoldingDelegations, bool>> associationsPredicate,
        int pageNumber,
        int pageSize,
        Expression<Func<CountyParishHoldingDelegations, TOrderBy>> orderBy,
        bool orderByDescending,
        CancellationToken cancellationToken = default)
    {
        if (HoldingAssignmentsFilter == null)
        {
            throw new InvalidOperationException("Holding assignments filter must be provided for this operation");
        }

        logger.LogInformation("Getting list of delegations for delegator");

        var primaryEntity = await readOnlyContext.UserAccounts
            .FirstOrDefaultAsync(primaryPredicate, cancellationToken);

        if (primaryEntity == null)
        {
            throw new NotFoundException("User account not found.");
        }

        var delegationEntities = readOnlyContext.Entry(primaryEntity)
            .Collection(p => p.ApplicationUserAccountHoldingAssignments)
            .Query()
            .Where(HoldingAssignmentsFilter)
            .Select(p => p.CountyParishHolding)
            .SelectMany(cphEntity => cphEntity.DelegationsCountyParishHoldings);

        var filteredDelegationEntities = delegationEntities
            .Include(p => p.CountyParishHolding)
            .Include(p => p.DelegatingUser)
            .Include(p => p.DelegatedUser)
            .Include(p => p.DelegatedUserRole)
            .Where(delegation => delegation.DelegatedUser != null)
            .Where(associationsPredicate);

        return await filteredDelegationEntities.ToPaged(pageNumber, pageSize, orderBy, orderByDescending, cancellationToken);
    }
}
