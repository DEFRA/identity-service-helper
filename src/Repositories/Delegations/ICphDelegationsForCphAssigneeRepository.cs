// <copyright file="ICphDelegationsForCphAssigneeRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites.Associations;

public interface ICphDelegationsForCphAssigneeRepository : IPageableAssociations<UserAccounts, CountyParishHoldingDelegations>
{
    ICphDelegationsForCphAssigneeRepository WithHoldingAssignmentsFilter(Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> holdingAssignmentsFilter);
}
