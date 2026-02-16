// <copyright file="OperatorIdMiddleware.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Middleware;

using Defra.Identity.Requests.MetaData;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class OperatorIdMiddleware(ILogger<OperatorIdMiddleware> logger) : JsonErrorMiddleware
{
    public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            var endpoint = context.GetEndpoint();

            // Minimal APIs won't have ControllerActionDescriptor metadata we use an explicit endpoint marker instead.
            var requiresOperatorId =
                endpoint?.Metadata.GetMetadata<RequiresOperatorId>() is not null;
            if (requiresOperatorId)
            {
                var headers = context.Request.Headers;
                var operatorId = headers.TryGetValue(RequestHeaderNames.OperatorId, out var oid) ? oid.ToString() : null;

                if (string.IsNullOrWhiteSpace(operatorId))
                {
                    await WriteJsonErrorAsync(
                        context,
                        statusCode: StatusCodes.Status400BadRequest,
                        code: "missing_header",
                        message: $"Header {RequestHeaderNames.OperatorId} is required.",
                        details: new { header = $"{RequestHeaderNames.OperatorId}" });
                    return;
                }

                if (!Guid.TryParse(operatorId, out _))
                {
                    await WriteJsonErrorAsync(
                        context,
                        statusCode: StatusCodes.Status400BadRequest,
                        code: "invalid_header",
                        message: $"Header {RequestHeaderNames.OperatorId} must be a valid GUID.",
                        details: new { header = $"{RequestHeaderNames.OperatorId}" });
                    return;
                }
            }

            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {MiddlewareName}", nameof(OperatorIdMiddleware));
            throw;
        }
    }
}
