// <copyright file="ApplicationEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Applications;

using Defra.Identity.Requests;
using Defra.Identity.Requests.Applications.Commands.Create;
using Defra.Identity.Requests.Applications.Commands.Update;
using Defra.Identity.Requests.Applications.Queries;
using Defra.Identity.Requests.Filters;
using Defra.Identity.Requests.MetaData;
using Defra.Identity.Services.Applications;
using Microsoft.AspNetCore.Mvc;

public static class ApplicationEndpoints
{
    public static void UseApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Applications, GetAll);

        app.MapGet(RouteNames.Applications + "/{id:guid}", Get)
            .WithName(RouteNames.Applications)
            .Produces<Responses.Applications.Application>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status404NotFound);

        app.MapPut(RouteNames.Applications + "/{id:guid}", Put)
            .AddEndpointFilter<ValidationFilter<UpdateApplication>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.Applications, Post)
            .AddEndpointFilter<ValidationFilter<CreateApplication>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces<Responses.Applications.Application>(StatusCodes.Status201Created, "application/json")
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        app.MapDelete(RouteNames.Applications + "/{id:guid}", Delete)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Post(
        CommandRequestHeaders headers,
        [FromBody] CreateApplication application,
        IApplicationService service)
    {
        application.OperatorId = headers.OperatorId;
        var result = await service.Create(application);

        return Results.CreatedAtRoute(
            routeName: RouteNames.Applications,
            routeValues: new { id = result.Id },
            value: result);
    }

    private static async Task<IResult> Put(
        CommandRequestHeaders headers,
        [FromRoute] Guid id,
        [FromBody] UpdateApplication application,
        IApplicationService service)
    {
        try
        {
            application.Id = id;
            application.OperatorId = headers.OperatorId;

            var result = await service.Update(application);
            return Results.Ok(result);
        }
        catch (NullReferenceException nex)
        {
            return Results.NotFound(nex.Message);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> Get(
        QueryRequestHeaders headers,
        [AsParameters] GetApplicationById request,
        IApplicationService service)
    {
        var application = await service.Get(request);

        return Results.Ok(application);
    }

    private static async Task<IResult> GetAll(
        QueryRequestHeaders headers,
        [AsParameters] GetApplications request,
        IApplicationService service)
    {
        var applications = await service.GetAll(request);

        return Results.Ok(applications);
    }

    private static async Task<IResult> Delete(
        CommandRequestHeaders headers,
        [FromRoute] Guid id,
        IApplicationService service)
    {
        var deleted = await service.Delete(id, headers.OperatorId);

        return Results.NoContent();
    }
}
