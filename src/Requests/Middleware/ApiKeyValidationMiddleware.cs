// <copyright file="ApiKeyValidationMiddleware.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class ApiKeyValidationMiddleware(string apiKey, ILogger<ApiKeyValidationMiddleware> logger) : JsonErrorMiddleware
{
    private readonly string apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

    public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            var headers = context.Request.Headers;

            // 1. Check API Key
            var headerKey = headers.TryGetValue(RequestHeaderNames.ApiKey, out var key) ? key.ToString() : null;
            if (string.IsNullOrWhiteSpace(key))
            {
                await WriteJsonErrorAsync(
                    context,
                    statusCode: StatusCodes.Status400BadRequest,
                    code: "Invalid header",
                    message: $"Header {RequestHeaderNames.ApiKey} is required.",
                    details: new { header = $"{RequestHeaderNames.ApiKey}" });
                return;
            }

            if (!string.Equals(key, this.apiKey, StringComparison.Ordinal))
            {
                await WriteJsonErrorAsync(
                    context,
                    statusCode: StatusCodes.Status400BadRequest,
                    code: "Invalid Api Key",
                    message: $"Header {RequestHeaderNames.ApiKey} is not valid.",
                    details: new { header = $"{RequestHeaderNames.ApiKey}" });
                return;
            }

            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {MiddlewareName}", nameof(ApiKeyValidationMiddleware));
            throw;
        }
    }
}
