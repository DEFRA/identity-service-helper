// <copyright file="FiltersLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Filters;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;

public static class FiltersLibrary
{
    public static Expression<Func<UserAccounts, bool>> UserAccountsNotDeletedFilter = holdingAssignment => holdingAssignment.DeletedAt == null;

    public static Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> HoldingAssignmentsNotDeletedFilter = holdingAssignment => holdingAssignment.DeletedAt == null;

    public static Expression<Func<CountyParishHoldings, bool>> CountyParishHoldingNotDeletedOrExpiredFilter =
        countyParishHolding => countyParishHolding.DeletedAt == null && countyParishHolding.ExpiredAt == null;

    public static Expression<Func<CountyParishHoldingDelegations, bool>> DelegationsNotDeletedOrExpiredFilter = delegation
        => delegation.DeletedAt == null && (delegation.ExpiresAt == null || DateTime.UtcNow < delegation.ExpiresAt);
}
