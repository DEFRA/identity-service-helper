// <copyright file="ICphDelegatesForDelegatorRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites.Associations;

public interface ICphDelegatesForDelegatorRepository : IPageableAssociations<UserAccounts, UserAccounts>
{
    ICphDelegatesForDelegatorRepository WithHoldingAssignmentsFilter(Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> holdingAssignmentsFilter);

    ICphDelegatesForDelegatorRepository WithCountyParishHoldingsFilter(Expression<Func<CountyParishHoldings, bool>> countyParishHoldingsFilter);

    ICphDelegatesForDelegatorRepository WithDelegationsFilter(Expression<Func<CountyParishHoldingDelegations, bool>> holdingAssignmentsFilter);
}
