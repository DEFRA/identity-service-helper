// <copyright file="MockValidatorContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Test.Utilities.Validation;

using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using NSubstitute.Core;

[ExcludeFromCodeCoverage]
public class MockValidatorContext<T>
    where T : class
{
    private readonly IValidator<T> validator;

    private MockValidatorContext(IValidator<T> validator)
    {
        this.validator = validator;

        WithValidationFailures([]);
    }

    public ValidatorInterceptionContext<T> Calls { get; } = new();

    public static MockValidatorContext<T> CreateFor(IValidator<T> validator)
    {
        return new MockValidatorContext<T>(validator);
    }

    public MockValidatorContext<T> WithValidationFailures(List<ValidationFailure> errors)
    {
        validator.ValidateAsync(Arg.Any<T>(), Arg.Any<CancellationToken>())
            .Returns(SetResultDataForValidateAsync);

        return this;

        Task<ValidationResult> SetResultDataForValidateAsync(CallInfo callInfo)
        {
            var requestToValidate = callInfo.Arg<T>();

            var result = new ValidationResult() { Errors = errors, };

            Calls.LastValidateAsync = new ValidatorInterceptionInstance<T>(requestToValidate, result);

            return Task.FromResult(result);
        }
    }
}
