// <copyright file="GetAnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Species.Queries;

using System.ComponentModel;

public class GetAnimalSpecies
{
    [Description(OpenApiMetadata.AnimalSpecies.Id)]
    public string Id { get; set; }
}
