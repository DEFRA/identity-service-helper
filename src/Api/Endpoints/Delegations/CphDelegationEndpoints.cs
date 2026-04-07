// <copyright file="CphDelegationEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Delegations;

using System.Net.Mime;
using Defra.Identity.Requests;
using Defra.Identity.Requests.Delegations.Commands.Accept;
using Defra.Identity.Requests.Delegations.Commands.Create;
using Defra.Identity.Requests.Delegations.Commands.Delete;
using Defra.Identity.Requests.Delegations.Commands.Expire;
using Defra.Identity.Requests.Delegations.Commands.Reject;
using Defra.Identity.Requests.Delegations.Commands.Revoke;
using Defra.Identity.Requests.Delegations.Commands.Update;
using Defra.Identity.Requests.Delegations.Queries;
using Defra.Identity.Requests.Filters;
using Defra.Identity.Requests.MetaData;
using Defra.Identity.Services.Delegations;
using Microsoft.AspNetCore.Mvc;

public static class CphDelegationEndpoints
{
    public static void UseCphDelegationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Delegations, GetAll);

        app.MapGet(RouteNames.Delegations + "/{id:guid}", Get)
            .WithName(RouteNames.Delegations)
            .Produces<Responses.Delegations.CphDelegation>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.Delegations, Post)
            .AddEndpointFilter<ValidationFilter<CreateCphDelegation>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces<Responses.Delegations.CphDelegation>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        app.MapPut(RouteNames.Delegations + "/{id:guid}", Put)
            .AddEndpointFilter<OperationByIdMappingFilter<UpdateCphDelegationById>>()
            .AddEndpointFilter<ValidationFilter<UpdateCphDelegationById>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.Delegations + "/{id:guid}:accept", Accept)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapPost(RouteNames.Delegations + "/{id:guid}:reject", Reject)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapPost(RouteNames.Delegations + "/{id:guid}:revoke", Revoke)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapPost(RouteNames.Delegations + "/{id:guid}:expire", Expire)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapDelete(RouteNames.Delegations + "/{id:guid}", Delete)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
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

    private static async Task<IResult> Post(
        CommandRequestHeaders headers,
        [FromBody] CreateCphDelegation request,
        ICphDelegationsService service)
    {
        var result = await service.Create(request);

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
        [FromBody] UpdateCphDelegationById request,
        ICphDelegationsService service)
    {
        var result = await service.Update(request);

        return Results.Ok(result);
    }

    private static async Task<IResult> Accept(
        CommandRequestHeaders headers,
        [AsParameters] AcceptCphDelegationById request,
        ICphDelegationsService service)
    {
        await service.Accept(request);

        return Results.NoContent();
    }

    private static async Task<IResult> Reject(
        CommandRequestHeaders headers,
        [AsParameters] RejectCphDelegationById request,
        ICphDelegationsService service)
    {
        await service.Reject(request);

        return Results.NoContent();
    }

    private static async Task<IResult> Revoke(
        CommandRequestHeaders headers,
        [AsParameters] RevokeCphDelegationById request,
        ICphDelegationsService service)
    {
        await service.Revoke(request);

        return Results.NoContent();
    }

    private static async Task<IResult> Expire(
        CommandRequestHeaders headers,
        [AsParameters] ExpireCphDelegationById request,
        ICphDelegationsService service)
    {
        await service.Expire(request);

        return Results.NoContent();
    }

    private static async Task<IResult> Delete(
        CommandRequestHeaders headers,
        [AsParameters] DeleteCphDelegationById request,
        ICphDelegationsService service)
    {
        await service.Delete(request);

        return Results.NoContent();
    }
}
