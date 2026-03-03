// <copyright file="ApplicationEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.AnimalSpecies;

using Defra.Identity.Requests;
using Defra.Identity.Requests.Common.Queries;
using Defra.Identity.Requests.Filters;
using Defra.Identity.Requests.MetaData;
using Defra.Identity.Requests.Species.Commands;
using Defra.Identity.Requests.Species.Queries;
using Defra.Identity.Services.AnimalSpecies;

public static class AnimalSpeciesEndpoints
{
    public static void UseApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.AnimalSpecies, GetAll)
            .AddEndpointFilter<ValidationFilter<PagedQuery>>();

        app.MapGet(RouteNames.AnimalSpecies + "/{id:string}", Get)
            .WithName(RouteNames.AnimalSpecies)
            .Produces<Responses.Applications.Application>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.AnimalSpecies + "/{id:string}:toggle", Toggle)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Toggle(
        CommandRequestHeaders headers,
        [AsParameters] ToggleAnimalSpecies request,
        IAnimalSpeciesService service)
    {
        await service.Toggle(request, headers.OperatorId);

        return Results.NoContent();
    }

    private static async Task<IResult> Get(
        QueryRequestHeaders headers,
        [AsParameters] GetAnimalSpecies request,
        IAnimalSpeciesService service)
    {
        var application = await service.Get(request);

        return Results.Ok(application);
    }

    private static async Task<IResult> GetAll(
        QueryRequestHeaders headers,
        [AsParameters] GetAllAnimalSpecies request,
        IAnimalSpeciesService service)
    {
        var applications = await service.GetAll(request);

        return Results.Ok(applications);
    }
}
