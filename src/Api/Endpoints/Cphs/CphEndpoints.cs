// <copyright file="CphEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Cphs;

using System.Net.Mime;
using Defra.Identity.Api.Endpoints.Cphs.Factories;
using Defra.Identity.Requests;
using Defra.Identity.Requests.Common.Queries;
using Defra.Identity.Requests.Cphs.Commands;
using Defra.Identity.Requests.Cphs.Common;
using Defra.Identity.Requests.Cphs.Queries;
using Defra.Identity.Requests.Filters;
using Defra.Identity.Requests.MetaData;
using Defra.Identity.Responses.Common;
using Defra.Identity.Responses.Cphs;
using Defra.Identity.Services.Cphs;

public static class CphEndpoints
{
    public static void UseCphEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.CountyParishHoldings, GetAllPaged)
            .AddEndpointFilter<ValidationFilter<PagedQuery>>()
            .Produces<PagedResults<Cph>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);

        app.MapGet(RouteNames.CountyParishHoldings + "/{id:guid}", Get)
            .Produces<Cph>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(
                RouteNames.CountyParishHoldings + "/{county:int}/{parish:int}/{holding:int}",
                CphHandlerFactory.CreateCphNumberRerouteHandler<GetCphByCphId, GetCphByCphNumber>(Get))
            .AddEndpointFilter<ValidationFilter<IOperationByCphNumber>>()
            .Produces<Cph>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.CountyParishHoldings + "/{id:guid}:expire", Expire)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        app.MapPost(
                RouteNames.CountyParishHoldings + "/{county:int}/{parish:int}/{holding:int}:expire",
                CphHandlerFactory.CreateCphNumberRerouteHandler<ExpireCphByCphId, ExpireCphByCphNumber>(Expire))
            .WithMetadata(new RequiresOperatorId())
            .AddEndpointFilter<ValidationFilter<IOperationByCphNumber>>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        app.MapDelete(RouteNames.CountyParishHoldings + "/{id:guid}", Delete)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        app.MapDelete(
                RouteNames.CountyParishHoldings + "/{county:int}/{parish:int}/{holding:int}",
                CphHandlerFactory.CreateCphNumberRerouteHandler<DeleteCphByCphId, DeleteCphByCphNumber>(Delete))
            .WithMetadata(new RequiresOperatorId())
            .AddEndpointFilter<ValidationFilter<IOperationByCphNumber>>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(RouteNames.CountyParishHoldings + "/{id:guid}/users", GetAllCphUsersPaged)
            .AddEndpointFilter<ValidationFilter<PagedQuery>>()
            .Produces<PagedResults<CphUser>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(
                RouteNames.CountyParishHoldings + "/{county:int}/{parish:int}/{holding:int}/users",
                CphHandlerFactory.CreateCphNumberRerouteHandler<GetCphUsersByCphId, GetCphUsersByCphNumber>(GetAllCphUsersPaged))
            .AddEndpointFilter<ValidationFilter<IOperationByCphNumber>>()
            .AddEndpointFilter<ValidationFilter<PagedQuery>>()
            .Produces<PagedResults<CphUser>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetAllPaged(
        QueryRequestHeaders headers,
        [AsParameters] GetCphs request,
        ICphService service)
    {
        var pagedCphResults = await service.GetAllPaged(request);

        return Results.Ok(pagedCphResults);
    }

    private static async Task<IResult> Get(
        QueryRequestHeaders headers,
        [AsParameters] GetCphByCphId request,
        ICphService service)
    {
        var cph = await service.Get(request);

        return Results.Ok(cph);
    }

    private static async Task<IResult> Expire(
        CommandRequestHeaders headers,
        [AsParameters] ExpireCphByCphId request,
        ICphService service)
    {
        await service.Expire(request, headers.OperatorId);

        return Results.NoContent();
    }

    private static async Task<IResult> Delete(
        CommandRequestHeaders headers,
        [AsParameters] DeleteCphByCphId request,
        ICphService service)
    {
        await service.Delete(request, headers.OperatorId);

        return Results.NoContent();
    }

    private static async Task<IResult> GetAllCphUsersPaged(
        QueryRequestHeaders headers,
        [AsParameters] GetCphUsersByCphId request,
        ICphService service)
    {
        var pagedCphUsersResults = await service.GetAllCphUsersPaged(request);

        return Results.Ok(pagedCphUsersResults);
    }
}
