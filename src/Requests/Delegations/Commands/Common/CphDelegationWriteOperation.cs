// <copyright file="CphDelegationWriteOperation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Commands.Common;

public abstract class CphDelegationWriteOperation
{
    public Guid OperatorId { get; set; } = Guid.Empty;

    public required Guid CountyParishHoldingId { get; set; }

    public required Guid DelegatingUserId { get; set; }

    public Guid? DelegatedUserId { get; set; }

    public required Guid DelegatedUserRoleId { get; set; }

    public required string DelegatedUserEmail { get; set; }
}
