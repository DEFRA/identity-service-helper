// <copyright file="ToggleAnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Species.Commands;

using Microsoft.AspNetCore.Mvc;

public class ToggleAnimalSpecies
{
    public string? Id { get; set; }

    public bool IsActive { get; set; }

    public ToggleAnimalSpecies WithId(string id)
    {
        this.Id = id;
        return this;
    }
}
