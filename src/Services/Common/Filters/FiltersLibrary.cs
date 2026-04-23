// <copyright file="FiltersLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Filters;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Extensions;

public static class FiltersLibrary
{
    public static class Users
    {
        public static readonly Expression<Func<UserAccounts, bool>> NotDeleted = holdingAssignment => holdingAssignment.DeletedAt == null;
    }

    public static class CphAssignments
    {
        public static readonly Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> NotDeleted = holdingAssignment => holdingAssignment.DeletedAt == null;
    }

    public static class Cphs
    {
        public static readonly Expression<Func<CountyParishHoldings, bool>> NotDeletedOrExpired =
            countyParishHolding => countyParishHolding.DeletedAt == null && countyParishHolding.ExpiredAt == null;
    }

    public static class CphDelegations
    {
        public static readonly Expression<Func<CountyParishHoldingDelegations, bool>> NotDeletedOrExpired = delegation
            => delegation.DeletedAt == null && (delegation.ExpiresAt == null || DateTime.UtcNow < delegation.ExpiresAt);

        public static readonly Expression<Func<CountyParishHoldingDelegations, bool>> DelegatingUserNotDeleted = delegation
            => delegation.DelegatingUser.DeletedAt == null;

        public static readonly Expression<Func<CountyParishHoldingDelegations, bool>> DelegatedUserNotDeleted = delegation
            => delegation.DelegatedUser != null && delegation.DelegatedUser.DeletedAt == null;

        public static readonly Expression<Func<CountyParishHoldingDelegations, bool>> CphNotDeletedOrExpired = delegation
            => delegation.CountyParishHolding.DeletedAt == null && delegation.CountyParishHolding.ExpiredAt == null;

        public static class Aggregates
        {
            public static readonly Expression<Func<CountyParishHoldingDelegations, bool>> DelegationAndReferencesNotDeletedOrExpired =
                NotDeletedOrExpired
                    .AndAlso(CphNotDeletedOrExpired)
                    .AndAlso(DelegatingUserNotDeleted)
                    .AndAlso(DelegatedUserNotDeleted);
        }
    }
}
