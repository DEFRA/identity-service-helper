// <copyright file="ApplicationsEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Applications;

using Defra.Identity.Mongo.Database.Documents;
using Defra.Identity.Repositories;

public static class ApplicationsEndpoints
{
    public static void UseApplicationsEndpoints(this IEndpointRouteBuilder app)
    {
      app.MapGet(RouteNames.Applications + "/{id:guid}", Get);
      app.MapPost(RouteNames.Applications, Create);
    }

    private static async Task<IResult> Get(
        Guid id,
        IRepository<Mongo.Database.Documents.Applications> service)
    {
        var matches = await service.Get(x => x.Id.Equals(id));
        return Results.Ok(matches);
    }

    private static async Task<IResult> Create(
        IRepository<Mongo.Database.Documents.Applications> service,
        Application app)
    {
        var application = new Applications
        {
            ClientId = Guid.NewGuid(),
            Description = app.Description,
            Name = app.Name,
            TenantName = app.Tenant,
            Timestamps = new Timestamps { CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
            Status = "Active",
        };
        await service.Create(application ?? throw new ArgumentNullException(nameof(application)));
        return Results.Ok(application);
    }

    private sealed record Application(
        string Name,
        string Description,
        string Tenant,
        Guid Client
    );
}
