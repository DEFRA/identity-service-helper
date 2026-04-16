// <copyright file="AnimalSpeciesEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Species;

using System.ComponentModel;
using Defra.Identity.Models.Requests;
using Defra.Identity.Models.Requests.Delegations.Commands;
using Defra.Identity.Models.Requests.Filters;
using Defra.Identity.Models.Requests.MetaData;
using Defra.Identity.Models.Requests.Species;
using Defra.Identity.Models.Requests.Species.Commands;
using Defra.Identity.Models.Requests.Species.Queries;
using Defra.Identity.Services.Species;
using Microsoft.AspNetCore.Mvc;

public static class AnimalSpeciesEndpoints
{
    public static void UseAnimalSpeciesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.AnimalSpecies, GetAllRoute)
            .WithName(OpenApiMetadata.GetAllRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.GetAllRoute.Summary)
            .WithDescription(OpenApiMetadata.GetAllRoute.Description)
            .Produces<IEnumerable<Models.Responses.Species.AnimalSpecies>>(StatusCodes.Status200OK, "application/json");

        app.MapGet(RouteNames.AnimalSpecies + "/{id}", GetByIdRoute)
            .WithName(OpenApiMetadata.GetByIdRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.GetByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.GetByIdRoute.Description)
            .Produces<Models.Responses.Species.AnimalSpecies>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.AnimalSpecies + "/{id}:toggle", ToggleByIdRoute)
            .WithName(OpenApiMetadata.ToggleByIdRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.ToggleByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.ToggleByIdRoute.Description)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> GetAllRoute(
        QueryRequestHeaders headers,
        [AsParameters] GetAllAnimalSpecies request,
        IAnimalSpeciesService service)
    {
        var applications = await service.GetAll(request);

        return Results.Ok(applications);
    }

    private static async Task<IResult> GetByIdRoute(
        QueryRequestHeaders headers,
        [AsParameters] GetAnimalSpecies request,
        IAnimalSpeciesService service)
    {
        var application = await service.Get(request);

        return Results.Ok(application);
    }

    private static async Task<IResult> ToggleByIdRoute(
        CommandRequestHeaders headers,
        [AsParameters] ToggleAnimalSpeciesById request,
        [FromBody] ToggleAnimalSpecies payload,
        IAnimalSpeciesService service)
    {
        payload.Id = request.Id;
        payload.OperatorId = headers.OperatorId;

        await service.Toggle(payload, headers.OperatorId);

        return Results.NoContent();
    }
}
