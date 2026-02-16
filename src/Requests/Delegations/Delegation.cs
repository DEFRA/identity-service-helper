// <copyright file="Delegation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations;

public abstract class Delegation
{
    public Guid ApplicationId { get; set; }

    public Guid UserId { get; set; }
}
