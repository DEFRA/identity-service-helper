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

    Task<AnimalSpecies> Get(GetAnimalSpecies request, CancellationToken cancellationToken = default);

    Task<AnimalSpecies> Toggle(ToggleAnimalSpecies request, Guid operatorId, CancellationToken cancellationToken = default);
}
