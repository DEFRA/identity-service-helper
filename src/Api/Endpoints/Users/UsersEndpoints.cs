// <copyright file="UsersEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Users;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Responses.Users;
using Defra.Identity.Services;
using Defra.Identity.Services.Users;

public static class UsersEndpoints
{
    public static void UseUsersEndpoints(this IEndpointRouteBuilder app)
    {
       // app.MapGet(RouteNames.Users, GetAll);
        app.MapGet(RouteNames.Users + "/{id:guid}", Get).Produces<Responses.Users.User>(StatusCodes.Status200OK, "application/json");

        app.MapPut(RouteNames.Users, Put);
    }

    private static async Task<IResult> Put(
        Requests.Users.User user,
        IUserService service)
    {
        var result = await service.Upsert(user);
        return Results.Ok(result);
    }

    private static async Task<IResult> Get(
        Guid id,
        IUserService service)
    {
        var matches = await service.Get(x => x.Id.Equals(id));
        return Results.Ok(matches);
    }
}
