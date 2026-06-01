// <copyright file="RoleEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Roles;

using System.Net.Mime;
using Defra.Identity.Models.Responses.Roles;
using Defra.Identity.Services.Roles;

public static class RoleEndpoints
{
    public static void UseRoleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Roles, GetAllRoute)
            .WithName(OpenApiMetadata.GetAllRoute.Name)
            .WithSummary(OpenApiMetadata.GetAllRoute.Summary)
            .WithDescription(OpenApiMetadata.GetAllRoute.Description)
            .WithTags(OpenApiMetadata.Tag)
            .Produces<IEnumerable<Role>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
    }

    private static async Task<IResult> GetAllRoute(IRoleService service)
    {
        var roles = await service.GetAll();

        return Results.Ok(roles);
    }
}
