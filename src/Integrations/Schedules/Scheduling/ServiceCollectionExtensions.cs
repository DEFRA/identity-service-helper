// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Scheduling;

using Defra.Identity.Scheduling.Configuration;
using Defra.Identity.Scheduling.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScheduling(
        this IServiceCollection services,
        IConfigurationRoot configuration)
    {
        services.AddJob<KeeperReferenceDataSchedulingOptions, KeeperReferenceDataJob>(configuration, "KeeperReferenceData");
        services.AddJob<MessagingSchedulingOptions, MessagingJob>(configuration, "Messaging");

        services.AddQuartzServer(options =>
        {
            // when shutting down, we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });

        return services;
    }

    private static IServiceCollection AddJob<TOption, TJob>(
        this IServiceCollection services,
        IConfigurationRoot configuration,
        string configSectionName)
        where TOption : BaseSchedulingOptions
        where TJob : IJob
    {
        // Bind options from configuration
        var section = configuration.GetRequiredSection($"Scheduling:{configSectionName}");
        services.Configure<TOption>(section);

        var cron = section["Cron"];
        if (string.IsNullOrWhiteSpace(cron))
        {
            throw new ArgumentException($"Cron expression is missing for {configSectionName} configuration section");
        }

        // Configure Quartz and register job + trigger
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey($"{configSectionName}Job");
            q.AddJob<TJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(t => t
                .ForJob(jobKey)
                .WithIdentity($"{configSectionName}-trigger")
                .WithCronSchedule(cron));
        });

        return services;
    }
}
