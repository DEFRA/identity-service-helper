// <copyright file="RulesLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations.Rules;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Builders.Predicates.Models;

public static class RulesLibrary
{
    public static class Existence
    {
        public static readonly EntityPredicate<CountyParishHoldingDelegations> NotSoftDeleted =
            new(delegation => delegation.DeletedAt == null, "Delegation must not be deleted");

        public static readonly EntityPredicate<CountyParishHoldingDelegations> NotExpired =
            new(delegation => delegation.ExpiresAt == null || DateTime.UtcNow < delegation.ExpiresAt, "Delegation must not have expired");
    }

    public static class Reference
    {
        public static class Descriptions
        {
            public const string CphMustExistNotDeletedOrExpired = "County parish holding must exist and not be deleted or expired";
            public const string DelegatingUserMustExistNotDeleted = "Delegating user must exist and not be deleted";
            public const string DelegatedUserMustExistNotDeleted = "Delegated user must exist and not be deleted";
            public const string DelegatedUserRoleMustExistNotDeleted = "Delegated user role must exist and not be deleted";
        }
    }

    public static class Business
    {
        public static readonly EntityPredicate<CountyParishHoldingDelegations> InvitationNotExpired =
            new(delegation => DateTime.UtcNow < delegation.InvitationExpiresAt, "Invitation must not have expired");

        public static readonly EntityPredicate<CountyParishHoldingDelegations> InvitationNotAccepted =
            new(delegation => delegation.InvitationAcceptedAt == null, "Invitation must not have been accepted");

        public static readonly EntityPredicate<CountyParishHoldingDelegations> InvitationNotRejected =
            new(delegation => delegation.InvitationRejectedAt == null, "Invitation must not have been rejected");

        public static readonly EntityPredicate<CountyParishHoldingDelegations> NotRevoked =
            new(delegation => delegation.RevokedAt == null, "Delegation must not have been revoked");
    }
}
