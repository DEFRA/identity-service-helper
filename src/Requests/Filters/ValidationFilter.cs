// <copyright file="ValidationFilter.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Filters;

using FluentValidation;
using Microsoft.AspNetCore.Http;

public class ValidationFilter<T> : IEndpointFilter
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var arg = context.Arguments.OfType<T>().FirstOrDefault();

        if (arg == null)
        {
            return Results.BadRequest("Invalid request.");
        }

        var validationResult = await _validator.ValidateAsync(arg);

        if (!validationResult.IsValid)
        {
            return Results.UnprocessableEntity(validationResult.ToDictionary());
        }

        return await next(context);
    }
}
