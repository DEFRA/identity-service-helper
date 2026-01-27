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
        app.MapGet(RouteNames.Users + "/{id:guid}", Get).Produces<UserAccount>(StatusCodes.Status200OK, "application/json");
        /*app.MapPut(RouteNames.Users, Put);
        app.MapPut(RouteNames.Users + "/{id:guid}", Put);*/
    }

    /*private static async Task<IResult> Put(
        UserAccount user,
        IUsers<UserAccount> service,
        Guid? id = null)
    {
        if (id.HasValue)
        {
            user.Id = id.Value;
        }

        if (user.Id == Guid.Empty)
        {
            return Results.BadRequest("User Id is required");
        }

        var existingUser = await service.Get(x => x.Id.Equals(user.Id));

        if (existingUser == null)
        {
            await service.Create(user);
            return Results.Created($"{RouteNames.Users}/{user.Id}", user);
        }

        await service.Update(user);
        return Results.Ok(user);
    }*/

    private static async Task<IResult> Get(
        Guid id,
        IUserService service)
    {
        var matches = await service.Get(id);
        return Results.Ok(matches);
    }
}
