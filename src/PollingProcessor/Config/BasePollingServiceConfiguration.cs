// <copyright file="BasePollingServiceConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.PollingProcessor.Config;

public class BasePollingServiceConfiguration
{
    public required string CronSchedule { get; init; }

    public required string ServiceType { get; init; }

    public required string InterfaceType { get; init; }

    public required string ConfigurationType { get; init; }

    public required string Description { get; init; }
}
