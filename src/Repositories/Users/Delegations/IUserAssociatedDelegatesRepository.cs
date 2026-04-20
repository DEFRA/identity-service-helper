// <copyright file="IUserAssociatedDelegatesRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites.Associations;

public interface IUserAssociatedDelegatesRepository : IPageableAssociations<UserAccounts, UserAccounts>
{
    IUserAssociatedDelegatesRepository WithHoldingAssignmentsFilter(Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> holdingAssignmentsFilter);

    IUserAssociatedDelegatesRepository WithCountyParishHoldingsFilter(Expression<Func<CountyParishHoldings, bool>> countyParishHoldingsFilter);

    IUserAssociatedDelegatesRepository WithDelegationsFilter(Expression<Func<CountyParishHoldingDelegations, bool>> holdingAssignmentsFilter);
}
