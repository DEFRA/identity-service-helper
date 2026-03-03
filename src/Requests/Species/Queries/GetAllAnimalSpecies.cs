// <copyright file="GetAllAnimalSpecies.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Species.Queries;

using Defra.Identity.Requests.Common.Queries;

public class GetAllAnimalSpecies : PagedQuery
{
    public bool IncludeInactive { get; set; } = false;
}
