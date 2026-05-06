// <copyright file="PermissionEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Permissions;

using System.Net.Mime;
using Defra.Identity.Api.Common.Factories;
using Defra.Identity.Api.Filters;
using Defra.Identity.Api.Middleware.Headers;
using Defra.Identity.Models.Requests.Common.Queries;
using Defra.Identity.Models.Requests.Cphs.Common;
using Defra.Identity.Models.Requests.Cphs.Queries;
using Defra.Identity.Models.Requests.Permissions.Queries;
using Defra.Identity.Models.Responses.Assignments;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Models.Responses.Permissions;
using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Services.Permissions;

public static class PermissionEndpoints
{
    public static void UsePermissionEndpoints(this IEndpointRouteBuilder app)
    {
        var scope = app.ServiceProvider.CreateScope();
        var cphNumberHandlerFactory = GetCphNumberHandlerFactory(scope);

        app.MapGet(RouteNames.CountyParishHoldings + "/{id:guid}/users", GetCphAssignmentsByIdRoute)
            .WithName(OpenApiMetadata.GetCphAssignmentsByIdRoute.Name)
            .WithTags(nameof(RouteNames.CountyParishHoldings))
            .WithSummary(OpenApiMetadata.GetCphAssignmentsByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.GetCphAssignmentsByIdRoute.Description)
            .AddEndpointFilter<ValidationFilter<PagedQuery>>()
            .Produces<PagedResults<CphAssignment>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(
                RouteNames.CountyParishHoldings + "/{county:int}/{parish:int}/{holding:int}/users",
                cphNumberHandlerFactory.CreateRerouteHandler<GetCphAssignmentsByCphId, GetCphAssigneesByCphNumber>(GetCphAssignmentsByIdRoute))
            .WithName(OpenApiMetadata.GetCphAssignmentsByCphNumberRoute.Name)
            .WithTags(nameof(RouteNames.CountyParishHoldings))
            .WithSummary(OpenApiMetadata.GetCphAssignmentsByCphNumberRoute.Summary)
            .WithDescription(OpenApiMetadata.GetCphAssignmentsByCphNumberRoute.Description)
            .AddEndpointFilter<ValidationFilter<IOperationByCphNumber>>()
            .AddEndpointFilter<ValidationFilter<PagedQuery>>()
            .Produces<PagedResults<CphAssignment>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(RouteNames.Users + "/{id:guid}/cphs", GetUserCphsRoute)
            .Produces<UserCphs>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapGet(RouteNames.Users + "/{id:guid}/delegates", GetUserDelegatesRoute)
            .AddEndpointFilter<ValidationFilter<PagedQuery>>()
            .Produces<PagedResults<User>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetCphAssignmentsByIdRoute(
        QueryRequestHeaders headers,
        [AsParameters] GetCphAssignmentsByCphId request,
        IPermissionsService service)
    {
        var pagedCphUsersResults = await service.GetCphAssignments(request);

        return Results.Ok(pagedCphUsersResults);
    }

    private static async Task<IResult> GetUserCphsRoute(
        QueryRequestHeaders headers,
        [AsParameters] GetUserCphsByUserId request,
        IPermissionsService service)
    {
        var user = await service.GetUserCphs(request);

        return Results.Ok(user);
    }

    private static async Task<IResult> GetUserDelegatesRoute(
        QueryRequestHeaders headers,
        [AsParameters] GetUserDelegatesById request,
        IPermissionsService service)
    {
        var user = await service.GetUserDelegates(request);

        return Results.Ok(user);
    }

    private static ICphNumberHandlerFactory<IPermissionsService> GetCphNumberHandlerFactory(IServiceScope scope)
        => scope.ServiceProvider.GetService<ICphNumberHandlerFactory<IPermissionsService>>() ?? throw new InvalidOperationException("CphNumberHandlerFactory is not registered.");
}
