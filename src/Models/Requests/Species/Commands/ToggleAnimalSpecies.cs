// <copyright file="ToggleAnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Species.Commands;

using System.ComponentModel;

public class ToggleAnimalSpecies
{
    [Description(OpenApiMetadata.AnimalSpecies.Id)]
    public string? Id { get; set; }

    [Description(OpenApiMetadata.Users.Id)]
    public Guid? OperatorId { get; set; }

    [Description(OpenApiMetadata.AnimalSpecies.IsActive)]
    public bool IsActive { get; set; }
}
