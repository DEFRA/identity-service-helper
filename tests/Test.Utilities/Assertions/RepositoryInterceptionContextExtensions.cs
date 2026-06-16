// <copyright file="RepositoryInterceptionContextExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Test.Utilities.Assertions;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Test.Utilities.Repository;

[ExcludeFromCodeCoverage]
[ShouldlyMethods]
public static class RepositoryInterceptionContextExtensions
{
    public static void ShouldHaveNoCalls<TEntity>(this RepositoryInterceptionContext<TEntity> instance)
    {
        instance.TotalCallCount.ShouldBe(0);
    }
}
