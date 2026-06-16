// <copyright file="ValidatorInterceptionInstance.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Test.Utilities.Validation;

using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

[ExcludeFromCodeCoverage]
public class ValidatorInterceptionInstance<T>
    where T : class
{
    public ValidatorInterceptionInstance(T? request, ValidationResult? result)
    {
        this.Request = request;
        this.Result = result;
    }

    public T? Request { get; }

    public ValidationResult? Result { get; }
}
