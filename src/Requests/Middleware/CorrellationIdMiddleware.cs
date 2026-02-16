// <copyright file="CorrellationIdMiddleware.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class CorrellationIdMiddleware(ILogger<CorrellationIdMiddleware> logger) : JsonErrorMiddleware
{
    public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            var headers = context.Request.Headers;
            var correlationId = headers.TryGetValue(RequestHeaderNames.CorrelationId, out var tid) ? tid.ToString() : null;
            correlationId = NormalizeHeaderValue(correlationId);
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                await WriteJsonErrorAsync(
                    context,
                    statusCode: StatusCodes.Status400BadRequest,
                    code: "missing_header",
                    message: $"Header {RequestHeaderNames.CorrelationId} is required.",
                    details: new { header = $"{RequestHeaderNames.CorrelationId}" });
                return;
            }

            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {MiddlewareName}", nameof(CorrellationIdMiddleware));
            throw;
        }
    }

    private static string? NormalizeHeaderValue(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var trimmed = value.Trim();

        // Treat empty quotes as "missing": "", '' (and also values with spaces like "  ").
        trimmed = trimmed.Trim('\"', '\'');

        trimmed = trimmed.Trim();

        return trimmed.Length == 0 ? null : trimmed;
    }
}
