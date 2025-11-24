// <copyright file="BasePollingServiceConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.PollingProcessor.Config;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class BasePollingServiceConfiguration
    : IValidatableObject
{
    public required string CronSchedule { get; init; }

    public required string ServiceType { get; init; }

    public required string InterfaceType { get; init; }

    public required string ConfigurationType { get; init; }

    public required string Description { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(CronSchedule))
        {
            yield return new ValidationResult("Cron Schedule cannot be empty", new[] { nameof(CronSchedule) });
        }

        if (string.IsNullOrEmpty(ServiceType))
        {
            yield return new ValidationResult("Service Type cannot be empty", new[] { nameof(ServiceType) });
        }

        if (string.IsNullOrEmpty(InterfaceType))
        {
            yield return new ValidationResult("Interface Type cannot be empty", new[] { nameof(InterfaceType) });
        }

        if (string.IsNullOrEmpty(ConfigurationType))
        {
            yield return new ValidationResult("Configuration Type cannot be empty", new[] { nameof(ConfigurationType) });
        }

        if (string.IsNullOrEmpty(Description))
        {
            yield return new ValidationResult("Description cannot be empty", new[] { nameof(Description) });
        }
    }
}
