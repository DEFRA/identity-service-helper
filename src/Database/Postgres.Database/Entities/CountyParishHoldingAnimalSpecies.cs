// <copyright file="CountyParishHoldingAnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

public class CountyParishHoldingAnimalSpecies
{
    public Guid CountyParishHoldingId { get; set; }

    public required CountyParishHoldings CountyParishHolding { get; set; }

    public string AnimalSpeciesId { get; set; }

    public required AnimalSpecies AnimalSpecies { get; set; }
}
