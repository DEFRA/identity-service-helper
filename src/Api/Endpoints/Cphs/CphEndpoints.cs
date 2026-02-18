// <copyright file="CphEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Cphs;

using Defra.Identity.Requests;
using Defra.Identity.Requests.Common.Queries;
using Defra.Identity.Requests.Cphs.Commands;
using Defra.Identity.Requests.Cphs.Queries;
using Defra.Identity.Requests.Filters;
using Defra.Identity.Requests.MetaData;
using Defra.Identity.Services.Cphs;
using Microsoft.AspNetCore.Mvc;

public static class CphEndpoints
{
    public static void UseCphEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.CountyParishHoldings, GetAllPaged)
            .AddEndpointFilter<ValidationFilter<PagedQueryBase>>();

        app.MapGet(RouteNames.CountyParishHoldings + "/{id:guid}", Get)
            .Produces<Responses.Cphs.Cph>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.CountyParishHoldings + "/{id:guid}:expire", Expire)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict);

        app.MapDelete(RouteNames.CountyParishHoldings + "/{id:guid}", Delete)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
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
        [AsParameters] GetCph request,
        ICphService service)
    {
        var cph = await service.Get(request);

        return Results.Ok(cph);
    }

    private static async Task<IResult> Expire(
        CommandRequestHeaders headers,
        [AsParameters] ExpireCph request,
        ICphService service)
    {
        await service.Expire(request, headers.OperatorId);

        return Results.NoContent();
    }

    private static async Task<IResult> Delete(
        CommandRequestHeaders headers,
        [AsParameters] DeleteCph request,
        ICphService service)
    {
        await service.Delete(request, headers.OperatorId);

        return Results.NoContent();
    }
}
