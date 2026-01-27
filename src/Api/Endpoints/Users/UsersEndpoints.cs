// <copyright file="UsersEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Users;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services;

public static class UsersEndpoints
{
    public static void UseUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Users, GetAll);
        app.MapGet(RouteNames.Users + "/{id:guid}", Get);
        app.MapPut(RouteNames.Users, Put);
        app.MapPut(RouteNames.Users + "/{id:guid}", Put);
    }

    private static async Task<IResult> Put(
        UserAccount user,
        IRepository<UserAccount> service,
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
    }

    private static async Task<IResult> GetAll(
        IRepository<UserAccount> service)
    {
        var matches = await service.GetAll();
        return Results.Ok(matches);
    }

    private static async Task<IResult> Get(
        Guid id,
        IRepository<UserAccount> service)
    {
        var matches = await service.Get(x => x.Id.Equals(id));
        return Results.Ok(matches);
    }
}
