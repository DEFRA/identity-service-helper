// <copyright file="IAnimalSpeciesService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Species;

using Defra.Identity.Requests.Species.Commands;
using Defra.Identity.Requests.Species.Queries;
using Defra.Identity.Responses.Species;

public interface IAnimalSpeciesService
{
    Task<List<AnimalSpecies>> GetAll(GetAllAnimalSpecies request, CancellationToken cancellationToken = default);

    Task<AnimalSpecies> Get(GetAnimalSpecies request, CancellationToken cancellationToken = default);

    Task<AnimalSpecies> Toggle(ToggleAnimalSpecies request, Guid operatorId, CancellationToken cancellationToken = default);
}
