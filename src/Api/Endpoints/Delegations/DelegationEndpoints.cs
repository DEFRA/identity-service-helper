// <copyright file="DelegationEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Delegations;

using System.Net.Mime;
using Defra.Identity.Requests;
using Defra.Identity.Requests.Delegations.Commands.Create;
using Defra.Identity.Requests.Delegations.Commands.Update;
using Defra.Identity.Requests.Delegations.Queries;
using Defra.Identity.Requests.Filters;
using Defra.Identity.Requests.MetaData;
using Defra.Identity.Services.Delegations;
using Microsoft.AspNetCore.Mvc;

public static class DelegationEndpoints
{
    public static void UseDelegationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Delegations, GetAll);

        app.MapGet(RouteNames.Delegations + "/{id:guid}", Get)
            .WithName(RouteNames.Delegations)
            .Produces<Responses.Delegations.CphDelegation>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPut(RouteNames.Delegations + "/{id:guid}", Put)
            .AddEndpointFilter<ValidationFilter<UpdateCphDelegationById>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.Delegations, Post)
            .AddEndpointFilter<ValidationFilter<CreateCphDelegation>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces<Responses.Delegations.CphDelegation>(StatusCodes.Status201Created, "application/json")
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        app.MapDelete(RouteNames.Delegations + "/{id:guid}", Delete)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Post(
        CommandRequestHeaders headers,
        [FromBody] CreateCphDelegation cphDelegation,
        ICphDelegationsService service)
    {
        cphDelegation.OperatorId = headers.OperatorId;
        var result = await service.Create(cphDelegation);

        return Results.CreatedAtRoute(
            routeName: RouteNames.Delegations,
            routeValues: new
            {
                id = result.Id,
            },
            value: result);
    }

    private static async Task<IResult> Put(
        CommandRequestHeaders headers,
        [FromRoute] Guid id,
        [FromBody] UpdateCphDelegationById cphDelegation,
        ICphDelegationsService service)
    {
        try
        {
            cphDelegation.Id = id;
            cphDelegation.OperatorId = headers.OperatorId;

            var result = await service.Update(cphDelegation);
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
        [AsParameters] GetCphDelegationById request,
        ICphDelegationsService service)
    {
        var delegation = await service.Get(request);

        return Results.Ok(delegation);
    }

    private static async Task<IResult> GetAll(
        QueryRequestHeaders headers,
        [AsParameters] GetCphDelegations request,
        ICphDelegationsService service)
    {
        var delegations = await service.GetAll(request);

        return Results.Ok(delegations);
    }

    private static async Task<IResult> Delete(
        CommandRequestHeaders headers,
        [FromRoute] Guid id,
        ICphDelegationsService service)
    {
        var deleted = await service.Delete(id, headers.OperatorId);

        return Results.NoContent();
    }
}
