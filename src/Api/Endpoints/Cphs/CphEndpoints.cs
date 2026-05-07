// <copyright file="CphEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Cphs;

using System.Net.Mime;
using Defra.Identity.Api.Common.Factories;
using Defra.Identity.Api.Filters;
using Defra.Identity.Api.MetaData;
using Defra.Identity.Api.Middleware.Headers;
using Defra.Identity.Models.Requests.Common.Queries;
using Defra.Identity.Models.Requests.Cphs.Commands;
using Defra.Identity.Models.Requests.Cphs.Common;
using Defra.Identity.Models.Requests.Cphs.Queries;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Models.Responses.Cphs;
using Defra.Identity.Services.Cphs;

public static class CphEndpoints
{
    public static void UseCphEndpoints(this IEndpointRouteBuilder app)
    {
        var scope = app.ServiceProvider.CreateScope();
        var cphNumberHandlerFactory = GetCphNumberHandlerFactory(scope);

        app.MapGet(RouteNames.CountyParishHoldings, GetAllPagedRoute)
            .WithName(OpenApiMetadata.GetAllPagedRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.GetAllPagedRoute.Summary)
            .WithDescription(OpenApiMetadata.GetAllPagedRoute.Description)
            .AddEndpointFilter<ValidationFilter<PagedQuery>>()
            .Produces<PagedResults<Cph>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);

        app.MapGet(RouteNames.CountyParishHoldings + "/{id:guid}", GetByIdRoute)
            .WithName(OpenApiMetadata.GetByIdRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.GetByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.GetByIdRoute.Description)
            .Produces<Cph>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(
                RouteNames.CountyParishHoldings + "/{county:int}/{parish:int}/{holding:int}",
                cphNumberHandlerFactory.CreateRerouteHandler<GetCphByCphId, GetCphByCphNumber>(GetByIdRoute))
            .WithName(OpenApiMetadata.GetByNumberRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.GetByNumberRoute.Summary)
            .WithDescription(OpenApiMetadata.GetByNumberRoute.Description)
            .AddEndpointFilter<ValidationFilter<IOperationByCphNumber>>()
            .Produces<Cph>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.CountyParishHoldings + "/{id:guid}:expire", ExpireByIdRoute)
            .WithName(OpenApiMetadata.ExpireByIdRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.ExpireByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.ExpireByIdRoute.Description)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        app.MapPost(
                RouteNames.CountyParishHoldings + "/{county:int}/{parish:int}/{holding:int}:expire",
                cphNumberHandlerFactory.CreateRerouteHandler<ExpireCphByCphId, ExpireCphByCphNumber>(ExpireByIdRoute))
            .WithName(OpenApiMetadata.ExpireByNumberRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.ExpireByNumberRoute.Summary)
            .WithDescription(OpenApiMetadata.ExpireByNumberRoute.Description)
            .WithMetadata(new RequiresOperatorId())
            .AddEndpointFilter<ValidationFilter<IOperationByCphNumber>>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        app.MapDelete(RouteNames.CountyParishHoldings + "/{id:guid}", DeleteByIdRoute)
            .WithName(OpenApiMetadata.DeleteByIdRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.DeleteByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.DeleteByIdRoute.Description)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        app.MapDelete(
                RouteNames.CountyParishHoldings + "/{county:int}/{parish:int}/{holding:int}",
                cphNumberHandlerFactory.CreateRerouteHandler<DeleteCphByCphId, DeleteCphByCphNumber>(DeleteByIdRoute))
            .WithName(OpenApiMetadata.DeleteByNumberRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.DeleteByNumberRoute.Summary)
            .WithDescription(OpenApiMetadata.DeleteByNumberRoute.Description)
            .WithMetadata(new RequiresOperatorId())
            .AddEndpointFilter<ValidationFilter<IOperationByCphNumber>>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetAllPagedRoute(
        [AsParameters] GetCphs request,
        ICphService service)
    {
        var pagedCphResults = await service.GetAllPaged(request);

        return Results.Ok(pagedCphResults);
    }

    private static async Task<IResult> GetByIdRoute(
        QueryRequestHeaders headers,
        [AsParameters] GetCphByCphId request,
        ICphService service)
    {
        var cph = await service.Get(request);

        return Results.Ok(cph);
    }

    private static async Task<IResult> ExpireByIdRoute(
        CommandRequestHeaders headers,
        [AsParameters] ExpireCphByCphId request,
        ICphService service)
    {
        await service.Expire(request, headers.OperatorId);

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteByIdRoute(
        CommandRequestHeaders headers,
        [AsParameters] DeleteCphByCphId request,
        ICphService service)
    {
        await service.Delete(request, headers.OperatorId);

        return Results.NoContent();
    }

    private static ICphNumberHandlerFactory<ICphService> GetCphNumberHandlerFactory(IServiceScope scope)
        => scope.ServiceProvider.GetService<ICphNumberHandlerFactory<ICphService>>() ?? throw new InvalidOperationException("CphNumberHandlerFactory is not registered.");
}
