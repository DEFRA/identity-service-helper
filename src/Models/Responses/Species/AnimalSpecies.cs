// <copyright file="AnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Species;

using System.ComponentModel;

public class AnimalSpecies
{
    [Description(OpenApiMetadata.AnimalSpecies.Id)]
    public required string Id { get; set; }

    [Description(OpenApiMetadata.AnimalSpecies.Name)]
    public required string Name { get; set; }

    [Description(OpenApiMetadata.AnimalSpecies.IsActive)]
    public required bool IsActive { get; set; }
}
