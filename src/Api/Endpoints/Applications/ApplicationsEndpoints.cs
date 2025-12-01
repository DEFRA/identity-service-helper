// <copyright file="ApplicationsEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Applications;

using Defra.Identity.Mongo.Database.Documents;
using Defra.Identity.Services;

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
        IRepository<Mongo.Database.Documents.Applications> service)
    {
        var application = new Mongo.Database.Documents.Applications
        {
            ClientId = "dfgsdfgfdg",
            Description = "Test Application",
            Name = "Test Application",
            TenantName = "Test Tenant",
            Timestamps = new Timestamps { CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
        };
        await service.Create(application ?? throw new ArgumentNullException(nameof(application)));
        return Results.Ok(application);
    }
}
