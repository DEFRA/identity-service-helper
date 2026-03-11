// <copyright file="GetAllAnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Species.Queries;

public class GetAllAnimalSpecies
{
    public string? IncludeInactive { get; set; } = null;
}
