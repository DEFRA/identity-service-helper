// <copyright file="EntityComparer.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Test.Utilities.Comparison;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Test.Utilities.Comparison.Models;

[ExcludeFromCodeCoverage]
public class EntityComparer
{
    private readonly object? compareSource;
    private readonly object? compareTarget;

    private EntityComparer(object? compareSource, object? compareTarget)
    {
        this.compareSource = compareSource;
        this.compareTarget = compareTarget;

        CompareTopLevelPropertiesWithoutObjectsOrEnumerables();
    }

    public List<PropertyVariance> VariancesAtTopLevelWithoutObjectsOrEnumerables { get; } = [];

    public static EntityComparer CreateFor(object? compareSource, object? compareTarget)
    {
        return new EntityComparer(compareSource, compareTarget);
    }

    private void CompareTopLevelPropertiesWithoutObjectsOrEnumerables()
    {
        if (object.Equals(compareSource, null) || object.Equals(compareTarget, null))
        {
            throw new InvalidOperationException("Cannot compare null objects");
        }

        if (compareSource.GetType() != compareTarget.GetType())
        {
            throw new InvalidOperationException("Cannot compare objects of different types");
        }

        var properties = compareSource.GetType().GetProperties();

        foreach (var property in properties)
        {
            if (!property.CanRead)
            {
                continue;
            }

            if (property.PropertyType != typeof(string))
            {
                if (property.PropertyType.IsClass)
                {
                    continue;
                }

                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    continue;
                }
            }

            var sourcePropertyValue = property.GetValue(compareSource);
            var targetPropertyValue = property.GetValue(compareTarget);

            if (!Equals(sourcePropertyValue, targetPropertyValue))
            {
                VariancesAtTopLevelWithoutObjectsOrEnumerables.Add(
                    new PropertyVariance(
                        property.Name,
                        sourcePropertyValue,
                        targetPropertyValue));
            }
        }
    }
}
