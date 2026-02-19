// <copyright file="Delegation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Responses.Delegations;

public class Delegation
{
    public Guid Id { get; set; }

    public Guid ApplicationId { get; set; }

    public Guid UserId { get; set; }
}
