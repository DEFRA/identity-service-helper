// <copyright file="CphDelegation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Responses.Delegations;

public class CphDelegation
{
    public required Guid Id { get; set; }

    public required Guid CountyParishHoldingId { get; set; }

    public required string CountyParishHoldingNumber { get; set; } = null!;

    public required Guid DelegatingUserId { get; set; }

    public required string DelegatingUserName { get; set; }

    public Guid? DelegatedUserId { get; set; }

    public required string? DelegatedUserName { get; set; }

    public required Guid DelegatedUserRoleId { get; set; }

    public required string DelegatedUserRoleName { get; set; }

    public required string DelegatedUserEmail { get; set; }

    public DateTime? InvitationExpiresAt { get; set; }

    public DateTime? InvitationAcceptedAt { get; set; }

    public DateTime? InvitationRejectedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public Guid? RevokedById { get; set; }

    public string? RevokedByName { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
