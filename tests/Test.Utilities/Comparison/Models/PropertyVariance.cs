// <copyright file="PropertyVariance.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Test.Utilities.Comparison.Models;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class PropertyVariance
{
    public PropertyVariance(string propertyName, object? sourceValue, object? targetValue)
    {
        PropertyName = propertyName;
        SourceValue = sourceValue;
        TargetValue = targetValue;
    }

    public string PropertyName { get; }

    public object? SourceValue { get; }

    public object? TargetValue { get; }
}
