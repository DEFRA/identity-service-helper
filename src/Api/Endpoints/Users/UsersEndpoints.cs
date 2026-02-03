// <copyright file="UsersEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Users;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Requests;
using Defra.Identity.Requests.Filters;
using Defra.Identity.Requests.Middleware;
using Defra.Identity.Requests.Users.Commands.Create;
using Defra.Identity.Requests.Users.Commands.Update;
using Defra.Identity.Requests.Users.Queries;
using Defra.Identity.Responses.Users;
using Defra.Identity.Services;
using Defra.Identity.Services.Users;
using Microsoft.AspNetCore.Mvc;

public static class UsersEndpoints
{
    public static void UseUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Users, GetAll);

        app.MapGet(RouteNames.Users + "/{id:guid}", Get)
            .Produces<Responses.Users.User>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status404NotFound);

        app.MapPut(RouteNames.Users + "/{id:guid}", Put)
            .AddEndpointFilter<ValidationFilter<UpdateUser>>()
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapPost(RouteNames.Users, Post)
            .AddEndpointFilter<ValidationFilter<CreateUser>>()
            .Produces<Responses.Users.User>(StatusCodes.Status201Created, "application/json")
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);
    }

    private static async Task<IResult> Post(
        IdentityRequestHeaders headers,
        [FromBody] CreateUser user,
        IUserService service)
    {
        user.OperatorId = headers.OperatorId;
        var result = await service.Create(user);

        return Results.CreatedAtRoute(
            routeName: RouteNames.Users,
            routeValues: new { id = result.Id },
            value: result);
    }

    private static async Task<IResult> Put(
        IdentityRequestHeaders headers,
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
        IdentityRequestHeaders headers,
        [AsParameters] GetUserById request,
        IUserService service)
    {
        var user = await service.Get(request);

        if (user == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(user);
    }

    private static async Task<IResult> GetAll(
        IdentityRequestHeaders headers,
        [AsParameters] GetUsers request,
        IUserService service)
    {
        var user = await service.GetAll(request);

        if (user == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(user);
    }
}
