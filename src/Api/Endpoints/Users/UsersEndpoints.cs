// <copyright file="UsersEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Users;

using System.Net.Mime;
using Defra.Identity.Requests;
using Defra.Identity.Requests.Filters;
using Defra.Identity.Requests.MetaData;
using Defra.Identity.Requests.Users.Commands.Create;
using Defra.Identity.Requests.Users.Commands.Update;
using Defra.Identity.Requests.Users.Commands.Validate;
using Defra.Identity.Requests.Users.Queries;
using Defra.Identity.Services.Users;
using Microsoft.AspNetCore.Mvc;

public static class UsersEndpoints
{
    public static void UseUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Users, GetAll);

        app.MapGet(RouteNames.Users + "/{id:guid}", Get)
            .WithName(RouteNames.Users)
            .Produces<Responses.Users.User>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPut(RouteNames.Users + "/{id:guid}", Put)
            .AddEndpointFilter<ValidationFilter<UpdateUser>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.Users, Post)
            .AddEndpointFilter<ValidationFilter<CreateUser>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces<Responses.Users.User>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        app.MapPost(string.Concat(RouteNames.Users, ":validate"), ValidateUser)
            .AddEndpointFilter<ValidationFilter<ValidateUser>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        app.MapDelete(RouteNames.Users + "/{id:guid}", Delete)
            .WithMetadata(new RequiresOperatorId())
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Post(
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

    private static async Task<IResult> Put(
        CommandRequestHeaders headers,
        [FromRoute] Guid id,
        [FromBody] UpdateUser user,
        IUserService service)
    {
        try
        {
            user.Id = id;
            user.OperatorId = headers.OperatorId;

            var result = await service.Update(user);
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
        [AsParameters] GetUserById request,
        IUserService service)
    {
        var user = await service.Get(request);

        return Results.Ok(user);
    }

    private static async Task<IResult> GetAll(
        QueryRequestHeaders headers,
        [AsParameters] GetUsers request,
        IUserService service)
    {
        var user = await service.GetAll(request);

        return Results.Ok(user);
    }

    private static async Task<IResult> Delete(
        CommandRequestHeaders headers,
        [FromRoute] Guid id,
        IUserService service)
    {
        var deleted = await service.Delete(id, headers.OperatorId);

        return Results.NoContent();
    }

    private static async Task<IResult> ValidateUser(
        CommandRequestHeaders headers,
        [FromBody] ValidateUser user,
        IUserService service)
    {
        var isValid = await service.Validate(headers.OperatorId, user.Email);

        return isValid ? Results.Ok() : Results.NotFound();
    }
}
