// <copyright file="CphDelegationWriteOperation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Delegations.Commands;

using System.ComponentModel;

public abstract class CphDelegationWriteOperation
{
    [Description(OpenApiMetadata.Cphs.Id)]
    public required Guid CountyParishHoldingId { get; set; }

    [Description(OpenApiMetadata.Delegations.DelegatingUserId)]
    public required Guid DelegatingUserId { get; set; }

    [Description(OpenApiMetadata.Delegations.DelegatedUserId)]
    public required Guid DelegatedUserId { get; set; }

    [Description(OpenApiMetadata.Delegations.DelegatedUserRoleId)]
    public required Guid DelegatedUserRoleId { get; set; }

    [Description(OpenApiMetadata.Delegations.DelegatedUserEmail)]
    public required string DelegatedUserEmail { get; set; }
}
