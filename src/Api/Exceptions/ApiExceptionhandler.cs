// <copyright file="ApiExceptionhandler.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Exceptions;

using Defra.Identity.Repositories.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public sealed class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, type) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found", "https://httpstatuses.com/404"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request", "https://httpstatuses.com/400"),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden", "https://httpstatuses.com/403"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "https://httpstatuses.com/500"),
        };

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
        };

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true; // exception handled
    }
}
