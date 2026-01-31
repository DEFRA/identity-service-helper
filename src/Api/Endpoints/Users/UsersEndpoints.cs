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
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        app.MapPost(RouteNames.Users, Post)
            .AddEndpointFilter<ValidationFilter<CreateUser>>();
    }

    private static async Task<IResult> Post(
        IdentityRequestHeaders headers,
        [FromBody] CreateUser user,
        IUserService service)
    {
        user.OperatorId = Guid.Parse(headers.OperatorId);
        var result = await service.Upsert(new UpdateUser
        {
            Email = user.EmailAddress,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DisplayName = user.DisplayName,
        });
        return Results.Ok(result);
    }

    private static async Task<IResult> Put(
        IdentityRequestHeaders headers,
        [FromRoute] Guid id,
        [FromBody] UpdateUser user,
        IUserService service)
    {
        user.Id = id;
        var result = await service.Update(user);

        if (result == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(result);
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
