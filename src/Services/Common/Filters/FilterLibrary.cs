// <copyright file="FilterLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Filters;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Extensions;
using ModelAnimalSpecies = Defra.Identity.Postgres.Database.Entities.AnimalSpecies;

#pragma warning disable SA1202 // ordering of the public and privates is important here
public static class FilterLibrary
{
    public static class Users
    {
        private static readonly Expression<Func<UserAccounts, bool>> NotSoftDeleted = holdingAssignment => holdingAssignment.DeletedAt == null;

        public static readonly Expression<Func<UserAccounts, bool>> Active = NotSoftDeleted;
    }

    public static class Cphs
    {
        public static readonly Expression<Func<CountyParishHoldings, bool>> NotSoftDeleted =
            countyParishHolding => countyParishHolding.DeletedAt == null;

        public static readonly Expression<Func<CountyParishHoldings, bool>> NotSoftDeletedOrExpired =
            countyParishHolding => countyParishHolding.DeletedAt == null && countyParishHolding.ExpiredAt == null;
    }

    public static class CphAssignments
    {
        private static readonly Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> NotSoftDeleted = holdingAssignment => holdingAssignment.DeletedAt == null;

        private static readonly Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> CphNotSoftDeletedOrExpired = holdingAssignment
            => holdingAssignment.CountyParishHolding.DeletedAt == null && holdingAssignment.CountyParishHolding.ExpiredAt == null;

        private static readonly Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> UserAccountNotSoftDeleted =
            holdingAssignment => holdingAssignment.UserAccount.DeletedAt == null;

        private static readonly Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> ApplicationNotSoftDeleted =
            holdingAssignment => holdingAssignment.Application.DeletedAt == null;

        public static readonly Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> Active =
            CphNotSoftDeletedOrExpired
                .AndAlso(UserAccountNotSoftDeleted)
                .AndAlso(ApplicationNotSoftDeleted)
                .AndAlso(NotSoftDeleted);
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

        private static readonly Expression<Func<CountyParishHoldingDelegations, bool>> ValidReferences =
            CphNotSoftDeletedOrExpired
                .AndAlso(DelegatingUserNotSoftDeleted)
                .AndAlso(DelegatedUserNotSoftDeleted);

        public static readonly Expression<Func<CountyParishHoldingDelegations, bool>> NotSoftDeleted = delegation
            => delegation.DeletedAt == null;

        public static readonly Expression<Func<CountyParishHoldingDelegations, bool>> NotSoftDeletedOrExpiredAndValidRefs =
            NotSoftDeletedOrExpired
                .AndAlso(ValidReferences);

        public static readonly Expression<Func<CountyParishHoldingDelegations, bool>> ActiveOrPending =
            NotSoftDeletedOrExpired
                .AndAlso(ValidReferences)
                .AndAlso(InvitationNotExpired)
                .AndAlso(InvitationNotRejected)
                .AndAlso(NotRevoked);
    }

    public static class AnimalSpecies
    {
        public static readonly Expression<Func<ModelAnimalSpecies, bool>> Active = animalSpecies
            => animalSpecies.IsActive;

        public static readonly Expression<Func<ModelAnimalSpecies, bool>> All = animalSpecies
            => true;
    }
}
#pragma warning restore SA1202
