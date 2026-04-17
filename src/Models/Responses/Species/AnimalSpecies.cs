// <copyright file="AnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Species;

using System.ComponentModel;

public class AnimalSpecies
{
    [Description(OpenApiMetadata.AnimalSpecies.Id)]
    public string Id { get; set; }

    [Description(OpenApiMetadata.AnimalSpecies.Name)]
    public string Name { get; set; }

    [Description(OpenApiMetadata.AnimalSpecies.IsActive)]
    public bool IsActive { get; set; }
}
