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
       // app.MapGet(RouteNames.Users, GetAll);
        app.MapGet(RouteNames.Users + "/{id:guid}", Get)
            .Produces<Responses.Users.User>(StatusCodes.Status200OK, "application/json")
            .Produces(StatusCodes.Status404NotFound);

        app.MapPut(RouteNames.Users, Put)
            .AddEndpointFilter<ValidationFilter<UpdateUser>>();

        app.MapPost(RouteNames.Users, Post)
            .AddEndpointFilter<ValidationFilter<CreateUser>>();
    }

    private static async Task<IResult> Post(
        UpdateUser user,
        IUserService service)
    {
        var result = await service.Upsert(user);
        return Results.Ok(result);
    }

    private static async Task<IResult> Put(
        Requests.Users.Commands.Update.UpdateUser user,
        IUserService service)
    {
        var result = await service.Upsert(user);
        return Results.Ok(result);
    }

    private static async Task<IResult> Get(
        IdentityRequestHeaders headers,
        [AsParameters] GetUser request,
        IUserService service)
    {
        var user = await service.Get(request);

        if (user == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(user);
    }
}
