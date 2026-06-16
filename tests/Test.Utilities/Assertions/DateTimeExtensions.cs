// <copyright file="DateTimeExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Test.Utilities.Assertions;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
[ShouldlyMethods]
public static class DateTimeExtensions
{
    public static void ShouldBeCloseToUtcNow(this DateTime dateTime)
    {
        dateTime.ShouldBe(DateTime.UtcNow, Tolerances.DateTimeTolerances.UtcNowComparisonTolerance);
    }

    public static void ShouldBeCloseToUtcNowAddDays(this DateTime dateTime, int daysToAdd)
    {
        dateTime.ShouldBe(DateTime.UtcNow.AddDays(daysToAdd), Tolerances.DateTimeTolerances.UtcNowComparisonTolerance);
    }
}
