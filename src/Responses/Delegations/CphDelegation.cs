// <copyright file="CphDelegation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Responses.Delegations;

public class CphDelegation
{
    public required Guid Id { get; init; }

    public required Guid CountyParishHoldingId { get; init; }

    public required string CountyParishHoldingNumber { get; set; }

    public required Guid DelegatingUserId { get; init; }

    public required string DelegatingUserName { get; init; }

    public required Guid? DelegatedUserId { get; init; }

    public required string? DelegatedUserName { get; init; }

    public required Guid DelegatedUserRoleId { get; init; }

    public required string DelegatedUserRoleName { get; init; }

    public required string DelegatedUserEmail { get; init; }

    public DateTime? InvitationExpiresAt { get; init; }

    public DateTime? InvitationAcceptedAt { get; init; }

    public DateTime? InvitationRejectedAt { get; init; }

    public DateTime? RevokedAt { get; init; }

    public Guid? RevokedById { get; init; }

    public string? RevokedByName { get; init; }

    public DateTime? ExpiresAt { get; init; }

    public bool Active { get; set; }
}
