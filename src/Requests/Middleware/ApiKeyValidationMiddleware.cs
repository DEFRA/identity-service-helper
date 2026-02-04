// <copyright file="ApiKeyValidationMiddleware.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Middleware;

using Microsoft.AspNetCore.Http;

public class ApiKeyValidationMiddleware(string apiKey) : JsonErrorMiddleware
{
    private readonly string apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

    public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var headers = context.Request.Headers;

        // 1. Check API Key
        var headerKey = headers.TryGetValue(IdentityHeaderNames.ApiKey, out var key) ? key.ToString() : null;
        if (string.IsNullOrWhiteSpace(key))
        {
            await WriteJsonErrorAsync(
                context,
                statusCode: StatusCodes.Status400BadRequest,
                code: "Invalid header",
                message: $"Header {IdentityHeaderNames.ApiKey} is required.",
                details: new { header = $"{IdentityHeaderNames.ApiKey}" });
            return;
        }

        if (!string.Equals(key, this.apiKey, StringComparison.Ordinal))
        {
            await WriteJsonErrorAsync(
                context,
                statusCode: StatusCodes.Status400BadRequest,
                code: "Invalid Api Key",
                message: $"Header {IdentityHeaderNames.ApiKey} is not valid.",
                details: new { header = $"{IdentityHeaderNames.ApiKey}" });
            return;
        }

        await next(context);
    }
}
