// <copyright file="CphDelegation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Delegations;

using System.ComponentModel;

public class CphDelegation
{
    [Description(OpenApiMetadata.Id)]
    public required Guid Id { get; set; }

    [Description(OpenApiMetadata.Id)]
    public required Guid CountyParishHoldingId { get; set; }

    [Description(OpenApiMetadata.CphId)]
    public required string CountyParishHoldingNumber { get; set; } = null!;

    [Description(OpenApiMetadata.Id)]
    public required Guid DelegatingUserId { get; set; }

    [Description(OpenApiMetadata.Delegations.DelegatingUserName)]
    public required string DelegatingUserName { get; set; }

    [Description(OpenApiMetadata.Id)]
    public Guid? DelegatedUserId { get; set; }

    [Description(OpenApiMetadata.Delegations.DelegatedUserName)]
    public required string? DelegatedUserName { get; set; }

    [Description(OpenApiMetadata.Id)]
    public required Guid DelegatedUserRoleId { get; set; }

    [Description(OpenApiMetadata.Delegations.DelegatedUserRoleName)]
    public required string DelegatedUserRoleName { get; set; }

    [Description(OpenApiMetadata.Delegations.DelegatedUserEmail)]
    public required string DelegatedUserEmail { get; set; }

    [Description(OpenApiMetadata.Delegations.InvitationExpiresAt)]
    public DateTime? InvitationExpiresAt { get; set; }

    [Description(OpenApiMetadata.Delegations.InvitationAcceptedAt)]
    public DateTime? InvitationAcceptedAt { get; set; }

    [Description(OpenApiMetadata.Delegations.InvitationRejectedAt)]
    public DateTime? InvitationRejectedAt { get; set; }

    [Description(OpenApiMetadata.Delegations.RevokedAt)]
    public DateTime? RevokedAt { get; set; }

    [Description(OpenApiMetadata.Id)]
    public Guid? RevokedById { get; set; }

    [Description(OpenApiMetadata.Delegations.RevokedByName)]
    public string? RevokedByName { get; set; }

    [Description(OpenApiMetadata.ExpiresAt)]
    public DateTime? ExpiresAt { get; set; }

    [Description(OpenApiMetadata.Delegations.DelegatingUserName)]
    public bool Active { get; set; }
}
