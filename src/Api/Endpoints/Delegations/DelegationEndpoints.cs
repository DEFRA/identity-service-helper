// <copyright file="DelegationEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Delegations;

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
            .Produces<Responses.Delegations.Delegation>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status404NotFound);

        app.MapPut(RouteNames.Delegations + "/{id:guid}", Put)
            .AddEndpointFilter<ValidationFilter<UpdateDelegation>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.Delegations, Post)
            .AddEndpointFilter<ValidationFilter<CreateDelegation>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces<Responses.Delegations.Delegation>(StatusCodes.Status201Created, "application/json")
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        app.MapDelete(RouteNames.Delegations + "/{id:guid}", Delete)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Post(
        CommandRequestHeaders headers,
        [FromBody] CreateDelegation delegation,
        IDelegationsService service)
    {
        delegation.OperatorId = headers.OperatorId;
        var result = await service.Create(delegation);

        return Results.CreatedAtRoute(
            routeName: RouteNames.Delegations,
            routeValues: new { id = result.Id },
            value: result);
    }

    private static async Task<IResult> Put(
        CommandRequestHeaders headers,
        [FromRoute] Guid id,
        [FromBody] UpdateDelegation delegation,
        IDelegationsService service)
    {
        try
        {
            delegation.Id = id;
            delegation.OperatorId = headers.OperatorId;

            var result = await service.Update(delegation);
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
        [AsParameters] GetDelegationById request,
        IDelegationsService service)
    {
        var delegation = await service.Get(request);

        return Results.Ok(delegation);
    }

    private static async Task<IResult> GetAll(
        QueryRequestHeaders headers,
        [AsParameters] GetDelegations request,
        IDelegationsService service)
    {
        var delegations = await service.GetAll(request);

        return Results.Ok(delegations);
    }

    private static async Task<IResult> Delete(
        CommandRequestHeaders headers,
        [FromRoute] Guid id,
        IDelegationsService service)
    {
        var deleted = await service.Delete(id, headers.OperatorId);

        return Results.NoContent();
    }
}
