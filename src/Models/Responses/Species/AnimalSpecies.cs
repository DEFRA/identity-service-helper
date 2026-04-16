// <copyright file="AnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Species;

public class AnimalSpecies
{
    public string Id { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }
}
