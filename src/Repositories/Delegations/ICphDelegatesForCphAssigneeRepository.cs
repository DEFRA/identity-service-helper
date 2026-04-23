// <copyright file="ICphDelegatesForCphAssigneeRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites.Associations;

public interface ICphDelegatesForCphAssigneeRepository : IPageableAssociations<UserAccounts, UserAccounts>
{
    ICphDelegatesForCphAssigneeRepository WithHoldingAssignmentsFilter(Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> holdingAssignmentsFilter);

    ICphDelegatesForCphAssigneeRepository WithCountyParishHoldingsFilter(Expression<Func<CountyParishHoldings, bool>> countyParishHoldingsFilter);

    ICphDelegatesForCphAssigneeRepository WithDelegationsFilter(Expression<Func<CountyParishHoldingDelegations, bool>> holdingAssignmentsFilter);
}
