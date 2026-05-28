// <copyright file="AnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

public class AnimalSpecies
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required bool IsActive { get; set; }

    public ICollection<CountyParishHoldingAnimalSpecies> CountyParishHoldingAnimalSpecies { get; set; } = new List<CountyParishHoldingAnimalSpecies>();

    public ICollection<UserAccountCountyParishHoldingAssignments> ApplicationUserAccountHoldingAssignments { get; set; } = new List<UserAccountCountyParishHoldingAssignments>();
}
