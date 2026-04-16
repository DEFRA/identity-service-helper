// <copyright file="ToggleAnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Species.Commands;

public class ToggleAnimalSpecies
{
    public string? Id { get; set; }

    public Guid? OperatorId { get; set; }

    public bool IsActive { get; set; }
}
