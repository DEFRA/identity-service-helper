// <copyright file="PingEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints;

using Defra.Identity.Requests;
using Defra.Identity.Requests.Middleware;
using Microsoft.Net.Http.Headers;

public static class PingEndpoints
{
    public static void UsePingEndpoints(this WebApplication app)
    {
        app.MapGet("/ping", (HttpContext http) =>
        {
            /*START_USER_CODE*/
            if (!IdentityRequestHeaders.TryGet(http, out var requestHeaders))
            {
                return Results.Problem(
                    title: "Missing request headers",
                    detail: "Identity request headers were not found on the current request context.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            return Results.Ok(new
            {
                CorrelationId = requestHeaders.CorrelationId,
                OperatorId = requestHeaders.OperatorId,
                ApiKey = requestHeaders.ApiKey,
            });
            /*END_USER_CODE*/
        });
    }
}
