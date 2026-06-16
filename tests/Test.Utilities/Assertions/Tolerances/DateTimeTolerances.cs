// <copyright file="DateTimeTolerances.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Test.Utilities.Assertions.Tolerances;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal static class DateTimeTolerances
{
    public static TimeSpan UtcNowComparisonTolerance { get; } = TimeSpan.FromSeconds(5);
}
