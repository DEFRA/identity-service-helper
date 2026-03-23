// <copyright file="HealthEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Health;

using Defra.Identity.Requests.MetaData;

public static class HealthEndpoints
{
    public static void UseHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Health, CalculateHealth)
            .WithMetadata(new IgnoreCorrelationIdCheck())
            .WithMetadata(new IgnoreApiKeyCheck());
    }

    private static async Task<IResult> CalculateHealth()
    {
        // TODO: Add database and dependent service checks
        return Results.Ok(new { status = "ok" });
    }
}
