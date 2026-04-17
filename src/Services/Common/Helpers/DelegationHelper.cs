// <copyright file="DelegationHelper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Helpers;

using Defra.Identity.Postgres.Database.Entities;

public static class DelegationHelper
{
    public static bool IsActiveDelegation(CountyParishHoldingDelegations entity)
    {
        var isDeleted = entity.DeletedAt != null;
        var hasExpired = entity.ExpiresAt != null && DateTime.UtcNow < entity.ExpiresAt;
        var rejectedOrRevoked = entity.InvitationRejectedAt != null || entity.RevokedAt != null;
        var isAccepted = entity.InvitationAcceptedAt != null;

        return !hasExpired && !isDeleted && !rejectedOrRevoked && isAccepted;
    }
}
