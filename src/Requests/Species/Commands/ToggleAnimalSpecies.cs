// <copyright file="ToggleAnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Species.Commands;

public class ToggleAnimalSpecies
{
    public Guid Id { get; set; }

    public bool IsActive { get; set; }
}
