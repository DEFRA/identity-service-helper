// <copyright file="ProfileEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Profiles;

using System.Net.Mime;
using Defra.Identity.Models.Requests.Profiles.Queries;
using Defra.Identity.Models.Responses.Profiles;
using Defra.Identity.Services.Profiles;

public static class ProfileEndpoints
{
    public static void UseProfileEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Users + "/{id:guid}/profile", GetUserProfileByIdRoute)
            .WithName(OpenApiMetadata.GetUserProfileByIdRoute.Name)
            .WithTags(nameof(RouteNames.Users))
            .WithSummary(OpenApiMetadata.GetUserProfileByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.GetUserProfileByIdRoute.Description)
            .Produces<UserProfile>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetUserProfileByIdRoute(
        [AsParameters] GetUserProfileByUserId request,
        IProfileService service)
    {
        var user = await service.GetUserProfile(request);

        return Results.Ok(user);
    }
}
