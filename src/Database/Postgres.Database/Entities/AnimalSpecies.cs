// <copyright file="AnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Configuration;

public class AnimalSpecies
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required bool IsActive { get; set; }

    public ICollection<ApplicationUserAccountHoldingAssignments> ApplicationUserAccountHoldingAssignments { get; set; } = new List<ApplicationUserAccountHoldingAssignments>();

    public ICollection<DelegationInvitations> DelegationInvitations { get; set; } = new List<DelegationInvitations>();
}
