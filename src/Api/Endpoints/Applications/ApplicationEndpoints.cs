// <copyright file="ApplicationEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Applications;

using System.Net.Mime;
using Defra.Identity.Api.Filters;
using Defra.Identity.Api.MetaData;
using Defra.Identity.Models.Requests.Applications.Commands;
using Defra.Identity.Models.Requests.Applications.Queries;
using Defra.Identity.Models.Responses.Applications;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.Applications;
using Microsoft.AspNetCore.Mvc;

public static class ApplicationEndpoints
{
    public static void UseApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Applications, GetAllRoute)
            .WithName(OpenApiMetadata.GetAllRoute.Name)
            .WithSummary(OpenApiMetadata.GetAllRoute.Summary)
            .WithDescription(OpenApiMetadata.GetAllRoute.Description)
            .WithTags(OpenApiMetadata.Tag)
            .Produces<IEnumerable<Application>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);

        app.MapGet(RouteNames.Applications + "/{id:guid}", GetByIdRoute)
            .WithName(OpenApiMetadata.GetByIdRoute.Name)
            .WithSummary(OpenApiMetadata.GetByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.GetByIdRoute.Description)
            .WithTags(OpenApiMetadata.Tag)
            .Produces<Application>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.Applications, PostRoute)
            .WithName(OpenApiMetadata.PostRoute.Name)
            .WithSummary(OpenApiMetadata.PostRoute.Summary)
            .WithDescription(OpenApiMetadata.PostRoute.Description)
            .WithTags(OpenApiMetadata.Tag)
            .AddEndpointFilter<ValidationFilter<CreateApplication>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces<Application>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        app.MapPut(RouteNames.Applications + "/{id:guid}", PutByIdRoute)
            .WithName(OpenApiMetadata.PutByIdRoute.Name)
            .WithSummary(OpenApiMetadata.PutByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.PutByIdRoute.Description)
            .WithTags(OpenApiMetadata.Tag)
            .AddEndpointFilter<OperationByGuidIdMappingFilter<UpdateApplicationByClientId>>()
            .AddEndpointFilter<ValidationFilter<UpdateApplicationByClientId>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapDelete(RouteNames.Applications + "/{id:guid}", DeleteByIdRoute)
            .WithName(OpenApiMetadata.DeleteByIdRoute.Name)
            .WithSummary(OpenApiMetadata.DeleteByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.DeleteByIdRoute.Description)
            .WithTags(OpenApiMetadata.Tag)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> PostRoute(
        [FromBody] CreateApplication request,
        IApplicationService service)
    {
        var result = await service.Create(request);

        return Results.CreatedAtRoute(
            routeName: RouteNames.Applications,
            routeValues: new
            {
                id = result.Id,
            },
            value: result);
    }

    private static async Task<IResult> GetAllRoute(
        [AsParameters] GetApplications request,
        IApplicationService service)
    {
        var applications = await service.GetAll(request);

        return Results.Ok(applications);
    }

    private static async Task<IResult> GetByIdRoute(
        [AsParameters] GetApplicationByClientId request,
        IApplicationService service)
    {
        var application = await service.Get(request);

        return Results.Ok(application);
    }

    private static async Task<IResult> PutByIdRoute(
        [FromBody] UpdateApplicationByClientId request,
        IApplicationService service)
    {
        try
        {
            var result = await service.Update(request);

            return Results.Ok(result);
        }
        catch (NotFoundException nex)
        {
            return Results.NotFound(nex.Message);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> DeleteByIdRoute(
        [AsParameters] DeleteApplicationByClientId request,
        IApplicationService service)
    {
        await service.Delete(request);

        return Results.NoContent();
    }
}
