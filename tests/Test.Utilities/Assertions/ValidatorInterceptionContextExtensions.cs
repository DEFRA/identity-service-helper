// <copyright file="ValidatorInterceptionContextExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Test.Utilities.Assertions;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Test.Utilities.Validation;

[ExcludeFromCodeCoverage]
[ShouldlyMethods]
public static class ValidatorInterceptionContextExtensions
{
    public static void ShouldHaveNoCalls<T>(this ValidatorInterceptionContext<T> instance)
        where T : class
    {
        instance.ValidateAsyncCallCount.ShouldBe(0);
    }
}
