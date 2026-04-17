// <copyright file="UsersEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Users;

using System.ComponentModel;
using System.Net.Mime;
using Defra.Identity.Api.Middleware.Headers;
using Defra.Identity.Models.Requests;
using Defra.Identity.Models.Requests.Filters;
using Defra.Identity.Models.Requests.MetaData;
using Defra.Identity.Models.Requests.Users;
using Defra.Identity.Models.Requests.Users.Commands;
using Defra.Identity.Models.Requests.Users.Queries;
using Defra.Identity.Responses.Users.Cphs.Aggregates;
using Defra.Identity.Services.Users;
using Microsoft.AspNetCore.Mvc;

public static class UsersEndpoints
{
    public static void UseUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Users, GetAllRoute)
            .WithName(OpenApiMetadata.GetAllRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.GetAllRoute.Summary)
            .WithDescription(OpenApiMetadata.GetAllRoute.Description)
            .Produces<IEnumerable<Models.Responses.Users.User>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);

        app.MapGet(RouteNames.Users + "/{id:guid}", GetByIdRoute)
            .WithName(OpenApiMetadata.GetByIdRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.GetByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.GetByIdRoute.Description)
            .Produces<Models.Responses.Users.User>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(RouteNames.Users + "/{id:guid}/cphs", GetUserCphsRoute)
            .Produces<UserCphs>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPut(RouteNames.Users + "/{id:guid}", PutByIdRoute)
            .WithName(OpenApiMetadata.PutByIdRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.PutByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.PutByIdRoute.Description)
            .AddEndpointFilter<ValidationFilter<UpdateUser>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.Users, PostRoute)
            .WithName(OpenApiMetadata.PostRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.PostRoute.Summary)
            .WithDescription(OpenApiMetadata.PostRoute.Description)
            .AddEndpointFilter<ValidationFilter<CreateUser>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces<Models.Responses.Users.User>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        app.MapDelete(RouteNames.Users + "/{id:guid}", DeleteByIdRoute)
            .WithName(OpenApiMetadata.DeleteByIdRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.DeleteByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.DeleteByIdRoute.Description)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> PostRoute(
        CommandRequestHeaders headers,
        [FromBody] CreateUser user,
        IUserService service)
    {
        user.OperatorId = headers.OperatorId;
        var result = await service.Create(user);

        return Results.CreatedAtRoute(
            routeName: RouteNames.Users,
            routeValues: new
            {
                id = result.Id,
            },
            value: result);
    }

    private static async Task<IResult> GetAllRoute(
        QueryRequestHeaders headers,
        [AsParameters] GetAllUsers request,
        IUserService service)
    {
        var user = await service.GetAll(request);

        return Results.Ok(user);
    }

    private static async Task<IResult> GetByIdRoute(
        QueryRequestHeaders headers,
        [AsParameters] GetUserById request,
        IUserService service)
    {
        var user = await service.Get(request);

        return Results.Ok(user);
    }

    private static async Task<IResult> PutByIdRoute(
        CommandRequestHeaders headers,
        [AsParameters] UpdateUserById request,
        [FromBody] UpdateUser payload,
        IUserService service)
    {
        try
        {
            payload.Id = request.Id;
            payload.OperatorId = headers.OperatorId;

            var result = await service.Update(payload);

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

    private static async Task<IResult> DeleteByIdRoute(
        CommandRequestHeaders headers,
        [AsParameters] DeleteUserById request,
        IUserService service)
    {
        await service.Delete(new DeleteUser()
            {
                OperatorId = headers.OperatorId,
                Id = request.Id,
            }
        );

        return Results.NoContent();
    }

    private static async Task<IResult> GetUserCphsRoute(
        QueryRequestHeaders headers,
        [AsParameters] GetUserCphsByUserId request,
        IUserService service)
    {
        var user = await service.GetUserCphs(request);

        return Results.Ok(user);
    }
}
