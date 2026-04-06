// <copyright file="OperationByIdMappingFilter.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Filters;

using Defra.Identity.Requests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public class OperationByIdMappingFilter<T> : IEndpointFilter
    where T : IOperationById
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<T>().FirstOrDefault();

        if (request == null)
        {
            return Results.BadRequest("Invalid request.");
        }

        if (Guid.TryParse(context.HttpContext.GetRouteValue("id")?.ToString(), out var id))
        {
            request.Id = id;

            return await next(context);
        }
        else
        {
            throw new InvalidOperationException("Unable to bind id for update");
        }
    }
}
