// <copyright file="IdentityRequestHeadersMiddleware.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Middleware;

using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

public sealed class IdentityRequestHeadersMiddleware(string apiKey) : IMiddleware
{
    private readonly string _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var headers = context.Request.Headers;

        // 1. Check API Key
        var apiKey = headers.TryGetValue(IdentityHeaderNames.ApiKey, out var key) ? key.ToString() : null;
        if (string.IsNullOrWhiteSpace(apiKey) || !string.Equals(apiKey, _apiKey, StringComparison.Ordinal))
        {
            await WriteJsonErrorAsync(
                context,
                statusCode: StatusCodes.Status400BadRequest,
                code: "Invalid header",
                message: $"Header {IdentityHeaderNames.ApiKey} is required.",
                details: new { header = $"{IdentityHeaderNames.ApiKey}" });
            return;
        }

        // 2) Get the Correllation Id
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

        // 3 Operator id
        var operatorId = headers.TryGetValue(IdentityHeaderNames.OperatorId, out var cid) ? cid.ToString() : null;
        if (string.IsNullOrWhiteSpace(operatorId))
        {
            await WriteJsonErrorAsync(
                context,
                statusCode: StatusCodes.Status400BadRequest,
                code: "invalid_header",
                message: $"Header {IdentityHeaderNames.OperatorId} has not been provided.",
                details: new { header = IdentityHeaderNames.OperatorId });
            return;
        }

        // 3) Store for downstream usage
        context.Items[IdentityRequestHeaders.ItemKey] = new IdentityRequestHeaders(
            CorrelationId: correlationId,
            OperatorId: operatorId,
            ApiKey: apiKey);

        // Optional: also set TraceIdentifier for consistent logging correlation
        context.TraceIdentifier = correlationId;

        await next(context);
    }

    private static async Task WriteJsonErrorAsync(
        HttpContext context,
        int statusCode,
        string code,
        string message,
        object? details = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = new
        {
            error = new
            {
                code,
                message,
                traceId = context.TraceIdentifier,
                path = context.Request.Path.Value,
                details,
            },
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
