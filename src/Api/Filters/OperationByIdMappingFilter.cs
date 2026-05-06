// <copyright file="OperationByIdMappingFilter.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Filters;

using Defra.Identity.Models.Requests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public class OperationByIdMappingFilter<T> : IEndpointFilter
    where T : class, IOperationById
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<T>().FirstOrDefault();

        if (request == null)
        {
            return Results.BadRequest("Invalid request.");
        }

        if (!Guid.TryParse(context.HttpContext.GetRouteValue("id")?.ToString(), out var id))
        {
            throw new InvalidOperationException("Unable to bind id for update");
        }

        request.Id = id;

        return await next(context);
    }
}
