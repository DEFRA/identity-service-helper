// <copyright file="DelegationMapper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Mappers;

using Defra.Identity.Models.Responses.Delegations;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Delegations.Helpers;

public static class DelegationMapper
{
    public static CphDelegation MapCphDelegationEntityToCphDelegation(CountyParishHoldingDelegations delegationEntity)
    {
        return new CphDelegation
        {
            Id = delegationEntity.Id,
            CountyParishHoldingId = delegationEntity.CountyParishHoldingId,
            CountyParishHoldingNumber = delegationEntity.CountyParishHolding.Identifier,
            DelegatingUserId = delegationEntity.DelegatingUserId,
            DelegatingUserName = delegationEntity.DelegatingUser.DisplayName,
            DelegatedUserId = delegationEntity.DelegatedUserId,
            DelegatedUserName = delegationEntity.DelegatedUser?.DisplayName,
            DelegatedUserEmail = delegationEntity.DelegatedUserEmail,
            DelegatedUserRoleId = delegationEntity.DelegatedUserRoleId,
            DelegatedUserRoleName = delegationEntity.DelegatedUserRole.Name,
            InvitationExpiresAt = delegationEntity.InvitationExpiresAt,
            InvitationAcceptedAt = delegationEntity.InvitationAcceptedAt,
            InvitationRejectedAt = delegationEntity.InvitationRejectedAt,
            RevokedAt = delegationEntity.RevokedAt,
            ExpiresAt = delegationEntity.ExpiresAt,
            RevokedById = delegationEntity.RevokedById,
            RevokedByName = delegationEntity.RevokedByUser?.DisplayName,
            Active = DelegationsHelper.IsActiveDelegation(delegationEntity),
        };
    }
}
