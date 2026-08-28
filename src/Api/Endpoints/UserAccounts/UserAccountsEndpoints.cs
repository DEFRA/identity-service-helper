// <copyright file="UserAccountsEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.UserAccounts;

using System.Net.Mime;
using Defra.Identity.Api.Filters;
using Defra.Identity.Api.MetaData;
using Defra.Identity.Models.Requests.UserAccounts.Commands;
using Defra.Identity.Models.Responses.UserAccounts;
using Defra.Identity.Services.UserAccounts;
using Microsoft.AspNetCore.Mvc;

public static class UserAccountsEndpoints
{
    public static void UseUserAccountsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(RouteNames.EnsureUserAccount, EnsureRoute)
            .WithName(OpenApiMetadata.EnsureRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.EnsureRoute.Summary)
            .WithDescription(OpenApiMetadata.EnsureRoute.Description)
            .AddEndpointFilter<ValidationFilter<EnsureUserAccount>>()
            .WithMetadata(new RequiresOperatorId())
            .Produces<UserAccount>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces<UserAccount>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        app.MapGet(RouteNames.UserAccount + "/{sub}", GetBySubjectRoute)
            .WithName(OpenApiMetadata.GetBySubjectRoute.Name)
            .WithTags(OpenApiMetadata.Tag)
            .WithSummary(OpenApiMetadata.GetBySubjectRoute.Summary)
            .WithDescription(OpenApiMetadata.GetBySubjectRoute.Description)
            .Produces<UserAccount>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> EnsureRoute(
        [FromBody] EnsureUserAccount request,
        IUserAccountService service,
        CancellationToken cancellationToken)
    {
        var result = await service.EnsureUserAccount(request, cancellationToken);

        return result.Created
            ? Results.CreatedAtRoute(
                routeName: OpenApiMetadata.GetBySubjectRoute.Name,
                routeValues: new
                {
                    sub = result.Account.Subject,
                },
                value: result.Account)
            : Results.Ok(result.Account);
    }

    private static async Task<IResult> GetBySubjectRoute(
        string sub,
        IUserAccountService service,
        CancellationToken cancellationToken)
    {
        var account = await service.GetUserAccountBySubject(sub, cancellationToken);

        return Results.Ok(account);
    }
}
