// <copyright file="UpdateDelegation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Commands.Update;

public class UpdateDelegation : Delegation
{
    public Guid Id { get; set; }

    public Guid OperatorId { get; set; } = Guid.Empty;
}
