// <copyright file="RulesLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations.Rules;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Common.Strategy.Rules.Models;

[ExcludeFromCodeCoverage]
public static class RulesLibrary
{
    public static class Reference
    {
        public static class Descriptions
        {
            public const string CphMustExistNotDeletedOrExpired =
                "County parish holding must exist and not be deleted or expired";

            public const string DelegatingUserMustExistNotDeleted = "Delegating user must exist and not be deleted";
            public const string DelegatedUserMustExistNotDeleted = "Delegated user must exist and not be deleted";
            public const string DelegatedUserRoleMustExist = "Delegated user role must exist";
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

        public static readonly EntityPredicate<CountyParishHoldingDelegations> DelegationNotRevoked =
            new(delegation => delegation.RevokedAt == null, "Delegation must not have been revoked");

        public static readonly EntityPredicate<CountyParishHoldingDelegations> DelegationNotExpired =
            new(
                delegation => delegation.ExpiresAt == null || DateTime.UtcNow < delegation.ExpiresAt,
                "Delegation must not have expired");

        public static readonly EntityPredicate<CountyParishHoldingDelegations> DelegationNotSoftDeleted =
            new(delegation => delegation.DeletedAt == null, "Delegation must not be deleted");
    }
}
