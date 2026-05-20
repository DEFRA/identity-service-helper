// <copyright file="OperationByStringIdMappingFilter.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Filters;

using Defra.Identity.Models.Requests.Common;

public class OperationByStringIdMappingFilter<T>
    : IEndpointFilter
    where T : class, IOperationById<string>
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<T>().FirstOrDefault();

        if (request == null)
        {
            return Results.BadRequest("Invalid request.");
        }

        var id = context.HttpContext.GetRouteValue("id")?.ToString() ?? throw new InvalidOperationException("Unable to bind string id for update");

        request.Id = id;

        return await next(context);
    }
}
