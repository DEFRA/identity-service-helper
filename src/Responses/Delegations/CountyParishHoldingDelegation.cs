// <copyright file="CountyParishHoldingDelegation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Responses.Delegations;

public class CountyParishHoldingDelegation
{
    public Guid Id { get; set; }

    public Guid CountyParishHoldingId { get; set; }

    public string CountyParishHolding { get; set; } = null!;

    public Guid DelegatingUserId { get; set; }

    public string DelegatingUserName { get; set; }

    public Guid? DelegatedUserId { get; set; }

    public string DelegatedUserName { get; set; }

    public Guid DelegatedUserRoleId { get; set; }

    public string DelegatedUserRoleName { get; set; }

    public DateTime InvitationExpiresAt { get; set; }

    public DateTime InvitationAcceptedAt { get; set; }

    public DateTime InvitationRejectedAt { get; set; }

    public DateTime RevokedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public Guid? RevokedById { get; set; }

    public string RevokedByName { get; set; }

    public DateTime ExpiredAt { get; set; }
}
