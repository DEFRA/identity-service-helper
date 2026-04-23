// <copyright file="ICphDelegationsForDelegatorRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Users.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Composites.Associations;

public interface ICphDelegationsForDelegatorRepository : IPageableAssociations<UserAccounts, CountyParishHoldingDelegations>
{
    ICphDelegationsForDelegatorRepository WithHoldingAssignmentsFilter(Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> holdingAssignmentsFilter);
}
