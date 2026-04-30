// <copyright file="FilterLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Permissions.Filters;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Extensions;

public static class FilterLibrary
{
    public static class Users
    {
        public static readonly Expression<Func<UserAccounts, bool>> NotSoftDeleted = holdingAssignment => holdingAssignment.DeletedAt == null;
    }

    public static class CphAssignments
    {
        public static readonly Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> NotSoftDeleted = holdingAssignment => holdingAssignment.DeletedAt == null;

        private static readonly Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> CphNotSoftDeletedOrExpired = holdingAssignment
            => holdingAssignment.CountyParishHolding.DeletedAt == null && holdingAssignment.CountyParishHolding.ExpiredAt == null;

        public static readonly Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> ActiveAssignment =
            CphNotSoftDeletedOrExpired
                .AndAlso(NotSoftDeleted);
    }

    public static class Cphs
    {
        public static readonly Expression<Func<CountyParishHoldings, bool>> NotSoftDeletedOrExpired =
            countyParishHolding => countyParishHolding.DeletedAt == null && countyParishHolding.ExpiredAt == null;
    }

    public static class CphDelegations
    {
        private static readonly Expression<Func<CountyParishHoldingDelegations, bool>> NotSoftDeletedOrExpired = delegation
            => delegation.DeletedAt == null && (delegation.ExpiresAt == null || DateTime.UtcNow < delegation.ExpiresAt);

        private static readonly Expression<Func<CountyParishHoldingDelegations, bool>> InvitationNotExpired = delegation
            => delegation.InvitationAcceptedAt != null || delegation.InvitationRejectedAt != null || DateTime.UtcNow < delegation.InvitationExpiresAt;

        private static readonly Expression<Func<CountyParishHoldingDelegations, bool>> InvitationNotRejected = delegation
            => delegation.InvitationRejectedAt == null;

        private static readonly Expression<Func<CountyParishHoldingDelegations, bool>> NotRevoked = delegation
            => delegation.RevokedAt == null;

        private static readonly Expression<Func<CountyParishHoldingDelegations, bool>> DelegatingUserNotSoftDeleted = delegation
            => delegation.DelegatingUser.DeletedAt == null;

        private static readonly Expression<Func<CountyParishHoldingDelegations, bool>> DelegatedUserNotSoftDeleted = delegation
            => delegation.DelegatedUser != null && delegation.DelegatedUser.DeletedAt == null;

        private static readonly Expression<Func<CountyParishHoldingDelegations, bool>> CphNotSoftDeletedOrExpired = delegation
            => delegation.CountyParishHolding.DeletedAt == null && delegation.CountyParishHolding.ExpiredAt == null;

        public static readonly Expression<Func<CountyParishHoldingDelegations, bool>> ActiveDelegation =
            InvitationNotExpired
                .AndAlso(InvitationNotRejected)
                .AndAlso(CphNotSoftDeletedOrExpired)
                .AndAlso(DelegatingUserNotSoftDeleted)
                .AndAlso(DelegatedUserNotSoftDeleted)
                .AndAlso(NotRevoked)
                .AndAlso(NotSoftDeletedOrExpired);
    }
}
