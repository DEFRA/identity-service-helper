// <copyright file="OperatorIdMiddleware.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Middleware;

using Microsoft.AspNetCore.Http;

public class OperatorIdMiddleware : JsonErrorMiddleware
{
    public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var headers = context.Request.Headers;
        var operatorId = headers.TryGetValue(IdentityHeaderNames.OperatorId, out var oid) ? oid.ToString() : null;

        if (string.IsNullOrWhiteSpace(operatorId))
        {
            await WriteJsonErrorAsync(
                context,
                statusCode: StatusCodes.Status400BadRequest,
                code: "missing_header",
                message: $"Header {IdentityHeaderNames.OperatorId} is required.",
                details: new { header = $"{IdentityHeaderNames.OperatorId}" });
            return;
        }

        if (!Guid.TryParse(operatorId, out _))
        {
            await WriteJsonErrorAsync(
                context,
                statusCode: StatusCodes.Status400BadRequest,
                code: "invalid_header",
                message: $"Header {IdentityHeaderNames.OperatorId} must be a valid GUID.",
                details: new { header = $"{IdentityHeaderNames.OperatorId}" });
            return;
        }

        await next(context);
    }
}
