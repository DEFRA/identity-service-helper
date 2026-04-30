// <copyright file="BaseSchedulingOptions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Scheduling.Configuration;

public abstract class BaseSchedulingOptions
{
    public string? Cron { get; set; }
}
