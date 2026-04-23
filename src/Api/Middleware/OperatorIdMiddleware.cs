// <copyright file="OperatorIdMiddleware.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Middleware;

using Defra.Identity.Api.Middleware.Headers;
using Defra.Identity.Models.Requests;
using Defra.Identity.Models.Requests.MetaData;
using Defra.Identity.Models.Requests.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class OperatorIdMiddleware(IOperatorIdService operatorIdService, ILogger<OperatorIdMiddleware> logger) : JsonErrorMiddleware
{
    public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // check if this maps to an endpoint. If not, just call the next middleware.
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await next(context);
            return;
        }

        try
        {
            // Minimal APIs won't have ControllerActionDescriptor metadata we use an explicit endpoint marker instead.
            var requiresOperatorId = endpoint.Metadata.GetMetadata<RequiresOperatorId>() is not null;
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
                        details: new
                        {
                            header = $"{RequestHeaderNames.OperatorId}",
                        });
                    return;
                }

                if (!Guid.TryParse(operatorId, out var parsedOperatorId))
                {
                    await WriteJsonErrorAsync(
                        context,
                        statusCode: StatusCodes.Status400BadRequest,
                        code: "invalid_header",
                        message: $"Header {RequestHeaderNames.OperatorId} must be a valid GUID.",
                        details: new
                        {
                            header = $"{RequestHeaderNames.OperatorId}",
                        });
                    return;
                }

                operatorIdService.OperatorId = parsedOperatorId;
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
