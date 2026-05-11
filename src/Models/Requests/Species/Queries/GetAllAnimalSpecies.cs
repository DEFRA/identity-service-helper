// <copyright file="GetAllAnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Species.Queries;

using System.ComponentModel;

public class GetAllAnimalSpecies
{
    [Description(OpenApiMetadata.IncludeInactive)]
    public string? IncludeInactive { get; set; } = null;
}
