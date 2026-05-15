// <copyright file="AnimalSpeciesService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Species;

using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Species.Commands;
using Defra.Identity.Models.Requests.Species.Queries;
using Defra.Identity.Repositories.Species;
using Defra.Identity.Services.Common;
using Defra.Identity.Services.Common.Builders.Strategy.Factories;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Common.Filters;
using Defra.Identity.Services.Common.Mappers;
using Microsoft.Extensions.Logging;
using ModelAnimalSpecies = Defra.Identity.Postgres.Database.Entities.AnimalSpecies;
using ResponseAnimalSpecies = Defra.Identity.Models.Responses.Species.AnimalSpecies;

public class AnimalSpeciesService : IAnimalSpeciesService
{
    private readonly IAnimalSpeciesRepository repository;
    private readonly IStrategyBuilderFactory<AnimalSpeciesService> strategyBuilderFactory;

    public AnimalSpeciesService(
        IAnimalSpeciesRepository repository,
        IOperatorContext operatorContext,
        IStrategyBuilderFactory<AnimalSpeciesService> strategyBuilderFactory,
        ILogger<AnimalSpeciesService> logger)
    {
        this.repository = repository;
        this.strategyBuilderFactory = strategyBuilderFactory;

        this.strategyBuilderFactory
            .WithDefaultLogger(logger)
            .WithDefaultOperatorContext(operatorContext)
            .WithDefaultEntityDescription(EntityDescriptions.AnimalSpecies);
    }

    public async Task<List<ResponseAnimalSpecies>> GetAll(
        GetAllAnimalSpecies request,
        CancellationToken cancellationToken = default)
    {
        var includeInactiveInferred = IncludeInactiveInferred(request);
        var animalSpeciesFilter = includeInactiveInferred ? FilterLibrary.AnimalSpecies.All : FilterLibrary.AnimalSpecies.Active;

        return await strategyBuilderFactory.BuildGetListStrategy<ModelAnimalSpecies>()
            .WithActionDescription($"Get all animal species, includeHidden: {includeInactiveInferred}")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithEntityFilter(animalSpeciesFilter)
            .ExecuteAndMap(AnimalSpeciesMapper.MapAnimalSpeciesEntityToAnimalSpecies);
    }

    public async Task<ResponseAnimalSpecies> Get(GetAnimalSpeciesById request, CancellationToken cancellationToken = default)
    {
        Expression<Func<ModelAnimalSpecies, bool>> animalSpeciesFilter = animalSpecies => animalSpecies.Id == request.Id;

        return await strategyBuilderFactory.BuildGetStrategy<ModelAnimalSpecies>()
            .WithActionDescription("Get animal species")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(animalSpeciesFilter)
            .ExecuteAndMap(AnimalSpeciesMapper.MapAnimalSpeciesEntityToAnimalSpecies);
    }

    public async Task<ResponseAnimalSpecies> Toggle(ToggleAnimalSpeciesById request, CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildUpdateStrategy<ModelAnimalSpecies>()
            .WithActionDescription("Toggle animal species")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(animalSpecies => request.Id == animalSpecies.Id)
            .WithUpdate(animalSpecies => { animalSpecies.IsActive = request.IsActive; })
            .ExecuteAndMap(AnimalSpeciesMapper.MapAnimalSpeciesEntityToAnimalSpecies);
    }

    private static bool IncludeInactiveInferred(GetAllAnimalSpecies request)
    {
        return request.IncludeInactive != null &&
               (request.IncludeInactive == string.Empty ||
                request.IncludeInactive.Equals("true", StringComparison.InvariantCultureIgnoreCase));
    }
}
