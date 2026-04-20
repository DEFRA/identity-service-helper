// <copyright file="UserAssociatedDelegatesRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using Defra.Identity.Repositories.Common.Exceptions;
using Microsoft.Extensions.Logging;

public class UserAssociatedDelegatesRepository(
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<UserAssociatedDelegatesRepository> logger) : IUserAssociatedDelegatesRepository
{
    private Expression<Func<ApplicationUserAccountHoldingAssignments, bool>>? HoldingAssignmentsFilter { get; set; }

    private Expression<Func<CountyParishHoldings, bool>>? CountyParishHoldingsFilter { get; set; }

    private Expression<Func<CountyParishHoldingDelegations, bool>>? DelegationsFilter { get; set; }

    public IUserAssociatedDelegatesRepository WithHoldingAssignmentsFilter(Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> holdingAssignmentsFilter)
    {
        HoldingAssignmentsFilter = holdingAssignmentsFilter;
        return this;
    }

    public IUserAssociatedDelegatesRepository WithCountyParishHoldingsFilter(Expression<Func<CountyParishHoldings, bool>> countyParishHoldingsFilter)
    {
        CountyParishHoldingsFilter = countyParishHoldingsFilter;
        return this;
    }

    public IUserAssociatedDelegatesRepository WithDelegationsFilter(Expression<Func<CountyParishHoldingDelegations, bool>> delegationsFilter)
    {
        DelegationsFilter = delegationsFilter;
        return this;
    }

    public async Task<PagedEntities<UserAccounts>> GetPaged<TOrderBy>(
        Expression<Func<UserAccounts, bool>> primaryPredicate,
        Expression<Func<UserAccounts, bool>> associationsPredicate,
        int pageNumber,
        int pageSize,
        Expression<Func<UserAccounts, TOrderBy>> orderBy,
        bool orderByDescending,
        CancellationToken cancellationToken = default)
    {
        if (HoldingAssignmentsFilter == null)
        {
            throw new InvalidOperationException("Holding assignments filter must be provided for this operation");
        }

        if (CountyParishHoldingsFilter == null)
        {
            throw new InvalidOperationException("County parish holdings filter must be provided for this operation");
        }

        if (DelegationsFilter == null)
        {
            throw new InvalidOperationException("Delegations filter must be provided for this operation");
        }

        logger.LogInformation("Getting list of delegates for user account");

        var primaryEntity = await readOnlyContext.UserAccounts
            .FirstOrDefaultAsync(primaryPredicate, cancellationToken);

        if (primaryEntity == null)
        {
            throw new NotFoundException("User account not found.");
        }

        var assignedCountyParishHoldingEntities = readOnlyContext.Entry(primaryEntity)
            .Collection(p => p.ApplicationUserAccountHoldingAssignments)
            .Query()
            .Where(HoldingAssignmentsFilter)
            .Select(p => p.CountyParishHolding);

        var delegationEntities = assignedCountyParishHoldingEntities
            .Where(CountyParishHoldingsFilter)
            .SelectMany(cphEntity => cphEntity.DelegationsCountyParishHoldings);

        var delegatedUserEntities = delegationEntities
            .Where(DelegationsFilter)
            .Where(delegation => delegation.DelegatedUser != null)
            .Select(delegation => delegation.DelegatedUser!);

        var filteredUserEntities = delegatedUserEntities
            .Where(associationsPredicate)
            .Distinct();

        return await filteredUserEntities.ToPaged(pageNumber, pageSize, orderBy, orderByDescending, cancellationToken);
    }
}
