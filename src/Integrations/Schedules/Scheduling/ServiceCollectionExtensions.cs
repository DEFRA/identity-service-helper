// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Scheduling;

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
        // Bind options from configuration
        var section = configuration.GetSection("Scheduling:KeeperReferenceData");
        services.Configure<KeeperReferenceDataOptions>(section);

        var cron = section["Cron"];

        // Configure Quartz and register job + trigger
        services.AddQuartz(q =>
        {
            if (!string.IsNullOrWhiteSpace(cron))
            {
                var jobKey = new JobKey("KeeperReferenceDataJob");
                q.AddJob<KeeperReferenceDataJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(t => t
                    .ForJob(jobKey)
                    .WithIdentity("KeeperReferenceDataJob-trigger")
                    .WithCronSchedule(cron));
            }
        });

        services.AddQuartzServer(options =>
        {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });
        return services;
    }
}
