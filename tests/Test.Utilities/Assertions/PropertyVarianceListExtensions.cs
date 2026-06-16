// <copyright file="PropertyVarianceListExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Test.Utilities.Assertions;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Test.Utilities.Comparison.Models;

[ExcludeFromCodeCoverage]
[ShouldlyMethods]
public static class PropertyVarianceListExtensions
{
    public static void ShouldOnlyHaveChanged(
        this List<PropertyVariance> propertyVariances,
        params string[] expectedPropertyNames)
    {
        var changedPropertyNames = propertyVariances.Select(x => x.PropertyName).ToList();

        changedPropertyNames.Count.ShouldBe(expectedPropertyNames.Length);

        var containsAllExpectedProperties =
            changedPropertyNames.All(changedPropertyName => expectedPropertyNames.Contains(changedPropertyName));

        containsAllExpectedProperties.ShouldBeTrue();
    }

    public static void ShouldHaveNoChanges(this List<PropertyVariance> propertyVariances)
    {
        propertyVariances.Count.ShouldBe(0);
    }
}
