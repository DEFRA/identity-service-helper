// <copyright file="OperatorIdMiddleware.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

public class OperatorIdMiddleware : JsonErrorMiddleware
{
    public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();

        // Only enforce OperatorId when the matched MVC action has a CommandRequestHeaders parameter.
        var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        var usesCommandRequestHeaders =
            actionDescriptor?.Parameters.Any(p => p.ParameterType == typeof(CommandRequestHeaders)) == true;

        if (!usesCommandRequestHeaders)
        {
            await next(context);
            return;
        }

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
