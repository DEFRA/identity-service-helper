// <copyright file="ValidatorInterceptionContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Test.Utilities.Validation;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class ValidatorInterceptionContext<T>
    where T : class
{
    public int ValidateAsyncCallCount { get; private set; }

    public List<ValidatorInterceptionInstance<T>?> ValidateAsyncLog { get; } = [];

    public ValidatorInterceptionInstance<T>? LastValidateAsync
    {
        get;

        set
        {
            field = value;
            ValidateAsyncCallCount++;
            ValidateAsyncLog.Add(value);
        }
    }
}
