// <copyright file="ExampleValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Api.Example.Validators;

using FluentValidation;
using Livestock.Auth.Api.Example.Models;

public class ExampleValidator : AbstractValidator<ExampleModel>
{
    /**
     * Example model validator.
     */
    public ExampleValidator()
    {
        RuleFor(model => model.Name)
            .Matches(@"^[\w\s]+$")
            .Length(3, 20)
            .WithMessage(
                "Name was not valid. Must be between 3 and 20 characters and contain only letters, numbers and whitespace.");

        RuleFor(model => model.Counter).GreaterThanOrEqualTo(0);
        RuleFor(model => model.Value).NotEmpty();
    }
}
