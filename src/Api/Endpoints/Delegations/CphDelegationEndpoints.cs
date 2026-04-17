// <copyright file="CphDelegationEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Delegations;

using System.ComponentModel;
using System.Net.Mime;
using Defra.Identity.Api.Middleware.Headers;
using Defra.Identity.Models.Requests;
using Defra.Identity.Models.Requests.Delegations;
using Defra.Identity.Models.Requests.Delegations.Commands;
using Defra.Identity.Models.Requests.Delegations.Queries;
using Defra.Identity.Models.Requests.Filters;
using Defra.Identity.Models.Requests.MetaData;
using Defra.Identity.Services.Delegations;
using Microsoft.AspNetCore.Mvc;

public static class CphDelegationEndpoints
{
    public static void UseCphDelegationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Delegations, GetAllRoute)
            .WithName(OpenApiMetadata.GetAll.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.GetAll.Summary)
            .WithDescription(OpenApiMetadata.GetAll.Description)
            .Produces<IEnumerable<Models.Responses.Delegations.CphDelegation>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);

        app.MapGet(RouteNames.Delegations + "/{id:guid}", GetByIdRoute)
            .WithName(OpenApiMetadata.GetById.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.GetById.Summary)
            .WithDescription(OpenApiMetadata.GetById.Description)
            .Produces<Models.Responses.Delegations.CphDelegation>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.Delegations, PostRoute)
            .WithName(OpenApiMetadata.Create.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.Create.Summary)
            .WithDescription(OpenApiMetadata.Create.Description)
            .AddEndpointFilter<ValidationFilter<CreateCphDelegation>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces<Models.Responses.Delegations.CphDelegation>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        app.MapPut(RouteNames.Delegations + "/{id:guid}", PutByIdRoute)
            .WithName(OpenApiMetadata.Update.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.Update.Summary)
            .WithDescription(OpenApiMetadata.Update.Description)
            .AddEndpointFilter<OperationByIdMappingFilter<UpdateCphDelegationById>>()
            .AddEndpointFilter<ValidationFilter<UpdateCphDelegationById>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.Delegations + "/{id:guid}:accept", AcceptByIdRoute)
            .WithName(OpenApiMetadata.Accept.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.Accept.Summary)
            .WithDescription(OpenApiMetadata.Accept.Description)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapPost(RouteNames.Delegations + "/{id:guid}:reject", RejectByIdRoute)
            .WithName(OpenApiMetadata.Reject.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.Reject.Summary)
            .WithDescription(OpenApiMetadata.Reject.Description)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapPost(RouteNames.Delegations + "/{id:guid}:revoke", RevokeByIdRoute)
            .WithName(OpenApiMetadata.Revoke.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.Revoke.Summary)
            .WithDescription(OpenApiMetadata.Revoke.Description)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapPost(RouteNames.Delegations + "/{id:guid}:expire", ExpireByIdRoute)
            .WithName(OpenApiMetadata.Expire.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.Expire.Summary)
            .WithDescription(OpenApiMetadata.Expire.Description)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        app.MapDelete(RouteNames.Delegations + "/{id:guid}", DeleteByIdRoute)
            .WithName(OpenApiMetadata.Delete.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.Delete.Summary)
            .WithDescription(OpenApiMetadata.Delete.Description)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetByIdRoute(
        QueryRequestHeaders headers,
        [AsParameters] GetCphDelegationById request,
        ICphDelegationsService service)
    {
        var delegation = await service.Get(request);

        return Results.Ok(delegation);
    }

    private static async Task<IResult> GetAllRoute(
        QueryRequestHeaders headers,
        [AsParameters] GetCphDelegations request,
        ICphDelegationsService service)
    {
        var delegations = await service.GetAll(request);

        return Results.Ok(delegations);
    }

    private static async Task<IResult> PostRoute(
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

    private static async Task<IResult> PutByIdRoute(
        CommandRequestHeaders headers,
        [FromBody] UpdateCphDelegationById request,
        ICphDelegationsService service)
    {
        var result = await service.Update(request);

        return Results.Ok(result);
    }

    private static async Task<IResult> AcceptByIdRoute(
        CommandRequestHeaders headers,
        [AsParameters] AcceptCphDelegationById request,
        ICphDelegationsService service)
    {
        await service.Accept(request);

        return Results.NoContent();
    }

    private static async Task<IResult> RejectByIdRoute(
        CommandRequestHeaders headers,
        [AsParameters] RejectCphDelegationById request,
        ICphDelegationsService service)
    {
        await service.Reject(request);

        return Results.NoContent();
    }

    private static async Task<IResult> RevokeByIdRoute(
        CommandRequestHeaders headers,
        [AsParameters] RevokeCphDelegationById request,
        ICphDelegationsService service)
    {
        await service.Revoke(request);

        return Results.NoContent();
    }

    private static async Task<IResult> ExpireByIdRoute(
        CommandRequestHeaders headers,
        [AsParameters] ExpireCphDelegationById request,
        ICphDelegationsService service)
    {
        await service.Expire(request);

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteByIdRoute(
        CommandRequestHeaders headers,
        [AsParameters] DeleteCphDelegationById request,
        ICphDelegationsService service)
    {
        await service.Delete(request);

        return Results.NoContent();
    }
}
