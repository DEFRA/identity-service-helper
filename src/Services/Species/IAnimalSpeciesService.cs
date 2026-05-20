// <copyright file="IAnimalSpeciesService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Species;

using Defra.Identity.Models.Requests.Species.Commands;
using Defra.Identity.Models.Requests.Species.Queries;
using Defra.Identity.Models.Responses.Species;

public interface IAnimalSpeciesService
{
    Task<List<AnimalSpecies>> GetAll(GetAllAnimalSpecies request, CancellationToken cancellationToken = default);

    Task<AnimalSpecies> Get(GetAnimalSpeciesById request, CancellationToken cancellationToken = default);

    Task<AnimalSpecies> Toggle(ToggleAnimalSpeciesById request, CancellationToken cancellationToken = default);
}
