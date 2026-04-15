// <copyright file="AnimalSpeciesService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Species;

using System.Linq.Expressions;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Species;
using Defra.Identity.Requests.Species.Commands;
using Defra.Identity.Requests.Species.Queries;
using Microsoft.Extensions.Logging;
using ModelAnimalSpecies = Defra.Identity.Postgres.Database.Entities.AnimalSpecies;
using ResponseAnimalSpecies = Defra.Identity.Responses.Species.AnimalSpecies;

public class AnimalSpeciesService
    : IAnimalSpeciesService
{
    private readonly IAnimalSpeciesRepository repository;
    private readonly ILogger<AnimalSpeciesService> logger;

    public AnimalSpeciesService(IAnimalSpeciesRepository repository, ILogger<AnimalSpeciesService> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    public async Task<List<ResponseAnimalSpecies>> GetAll(
        GetAllAnimalSpecies request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all animal species, includeHidden: {IncludeHidden}", request.IncludeInactive);
        Expression<Func<ModelAnimalSpecies, bool>> filter = x => IncludeInactiveInferred(request) || x.IsActive == true;

        var result = await repository.GetList(filter, cancellationToken);

        var speciesList = result.Select(x => new ResponseAnimalSpecies()
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive,
            })
            .ToList();

        return speciesList;
    }

    public async Task<ResponseAnimalSpecies> Get(GetAnimalSpecies request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting animal species by id {Id}", request.Id);
        Expression<Func<ModelAnimalSpecies, bool>> filter = x => x.Id == request.Id;

        var animalSpecies = await repository.GetSingle(filter, cancellationToken);
        if (animalSpecies == null)
        {
            logger.LogWarning("Animal species with id {Id} not found", request.Id);
            throw new NotFoundException("Animal species not found.");
        }

        return new ResponseAnimalSpecies()
        {
            Id = animalSpecies.Id,
            Name = animalSpecies.Name,
            IsActive = animalSpecies.IsActive,
        };
    }

    public async Task<ResponseAnimalSpecies> Toggle(ToggleAnimalSpecies request, Guid operatorId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating animal species with id {Id}", request.Id);
        var animalSpecies = await repository.GetSingle(x => x.Id.Equals(request.Id), cancellationToken);
        if (animalSpecies == null)
        {
            logger.LogWarning("Animal species with id {Id} not found for update", request.Id);
            throw new NotFoundException($"Animal species with id {request.Id} not found.");
        }

        animalSpecies.IsActive = request.IsActive;
        var updated = await repository.Update(animalSpecies, cancellationToken);

        return new ResponseAnimalSpecies
        {
            Id = updated.Id,
            Name = updated.Name,
            IsActive = updated.IsActive,
        };
    }

    private static bool IncludeInactiveInferred(GetAllAnimalSpecies request)
    {
        return request.IncludeInactive != null &&
               (request.IncludeInactive == string.Empty ||
                request.IncludeInactive.Equals("true", StringComparison.InvariantCultureIgnoreCase));
    }
}
