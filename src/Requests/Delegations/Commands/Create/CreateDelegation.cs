// <copyright file="CreateDelegation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Commands.Create;

public class CreateDelegation : Delegation
{
    public Guid OperatorId { get; set; } = Guid.Empty;
}
