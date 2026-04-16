// <copyright file="UserDelegatedCph.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Responses.Users.Cphs;

public class UserDelegatedCph
{
    public required Guid DelegationId { get; set; }

    public required Guid CountyParishHoldingId { get; init; }

    public required string CountyParishHoldingNumber { get; set; }

    public required Guid DelegatingUserId { get; init; }

    public required string DelegatingUserName { get; init; }

    public required Guid DelegatedUserRoleId { get; init; }

    public required string DelegatedUserRoleName { get; init; }

    public DateTime? InvitationExpiresAt { get; init; }

    public DateTime? InvitationAcceptedAt { get; init; }

    public DateTime? InvitationRejectedAt { get; init; }

    public DateTime? RevokedAt { get; init; }

    public Guid? RevokedById { get; init; }

    public string? RevokedByName { get; init; }

    public DateTime? ExpiresAt { get; init; }

    public bool Active { get; set; }
}
