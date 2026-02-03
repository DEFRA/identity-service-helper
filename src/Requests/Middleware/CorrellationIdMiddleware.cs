// <copyright file="CorrellationIdMiddleware.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Middleware;

using Microsoft.AspNetCore.Http;

public class CorrellationIdMiddleware : JsonErrorMiddleware
{
    public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var headers = context.Request.Headers;
        var correlationId = headers.TryGetValue(IdentityHeaderNames.CorrelationId, out var tid) ? tid.ToString() : null;
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            await WriteJsonErrorAsync(
                context,
                statusCode: StatusCodes.Status400BadRequest,
                code: "missing_header",
                message: $"Header {IdentityHeaderNames.CorrelationId} is required.",
                details: new { header = $"{IdentityHeaderNames.CorrelationId}" });
            return;
        }

        await next(context);
    }
}
