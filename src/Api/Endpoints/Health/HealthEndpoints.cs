// <copyright file="HealthEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Health;

using Defra.Identity.Models.Requests.MetaData;

public static class HealthEndpoints
{
    public static void UseHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Health, CalculateHealthRoute)
            .WithName(OpenApiMetadata.Get.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.Get.Summary)
            .WithDescription(OpenApiMetadata.Get.Description)
            .WithMetadata(new IgnoreCorrelationIdCheck())
            .WithMetadata(new IgnoreApiKeyCheck());
    }

    private static async Task<IResult> CalculateHealthRoute()
    {
        // TODO: Add database and dependent service checks
        return Results.Ok(new { status = "ok" });
    }
}
