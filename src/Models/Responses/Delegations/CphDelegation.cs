// <copyright file="CphDelegation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Delegations;

using System.ComponentModel;

public class CphDelegation
{
    [Description(OpenApiMetadata.Delegations.Id)]
    public required Guid Id { get; init; }

    [Description(OpenApiMetadata.Cphs.Id)]
    public required Guid CountyParishHoldingId { get; init; }

    [Description(OpenApiMetadata.Cphs.CphNumber)]
    public required string CountyParishHoldingNumber { get; set; }

    [Description(OpenApiMetadata.Delegations.DelegatingUserId)]
    public required Guid DelegatingUserId { get; init; }

    [Description(OpenApiMetadata.Delegations.DelegatingUserName)]
    public required string DelegatingUserName { get; init; }

    [Description(OpenApiMetadata.Delegations.DelegatedUserId)]
    public Guid? DelegatedUserId { get; init; }

    [Description(OpenApiMetadata.Delegations.DelegatedUserName)]
    public required string? DelegatedUserName { get; init; }

    [Description(OpenApiMetadata.Delegations.DelegatedUserRoleId)]
    public required Guid DelegatedUserRoleId { get; init; }

    [Description(OpenApiMetadata.Delegations.DelegatedUserRoleName)]
    public required string DelegatedUserRoleName { get; init; }

    [Description(OpenApiMetadata.Delegations.DelegatedUserEmail)]
    public required string DelegatedUserEmail { get; init; }

    [Description(OpenApiMetadata.Delegations.InvitationExpiresAt)]
    public DateTime? InvitationExpiresAt { get; init; }

    [Description(OpenApiMetadata.Delegations.InvitationAcceptedAt)]
    public DateTime? InvitationAcceptedAt { get; init; }

    [Description(OpenApiMetadata.Delegations.InvitationRejectedAt)]
    public DateTime? InvitationRejectedAt { get; init; }

    [Description(OpenApiMetadata.Delegations.RevokedAt)]
    public DateTime? RevokedAt { get; init; }

    [Description(OpenApiMetadata.Delegations.RevokedById)]
    public Guid? RevokedById { get; init; }

    [Description(OpenApiMetadata.Delegations.RevokedByName)]
    public string? RevokedByName { get; init; }

    [Description(OpenApiMetadata.Generic.ExpiresAt)]
    public DateTime? ExpiresAt { get; init; }

    [Description(OpenApiMetadata.Delegations.DelegatingUserName)]
    public required bool Active { get; set; }
}
