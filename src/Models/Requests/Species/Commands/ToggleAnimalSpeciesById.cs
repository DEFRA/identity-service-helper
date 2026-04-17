// <copyright file="ToggleAnimalSpeciesById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Species.Commands;

using System.ComponentModel;

public class ToggleAnimalSpeciesById
{
    [Description(OpenApiMetadata.AnimalSpecies.Id)]
    public string? Id { get; set; }
}
