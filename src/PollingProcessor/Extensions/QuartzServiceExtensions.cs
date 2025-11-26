// <copyright file="QuartzServiceExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

// ReSharper disable once CheckNamespace
namespace Defra.Identity.Extensions;

using System.ComponentModel.DataAnnotations;
using Defra.Identity.PollingProcessor.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

public static class QuartzServiceExtensions
{
    public static IServiceCollection AddPollingProcessorService(this IServiceCollection sc, IConfigurationRoot config)
    {
        var polledServices = config.GetRequiredSection("PolledServices");

        sc.AddQuartz(q =>
        {
            foreach (var serviceConfig in polledServices.GetChildren())
            {
                var baseConfig = serviceConfig.Get<BasePollingServiceConfiguration>();
                var context = new ValidationContext(baseConfig);
                var results = new List<ValidationResult>();
                if (!Validator.TryValidateObject(baseConfig, context, results, true))
                {
                    throw new InvalidOperationException($"Invalid configuration for service '{serviceConfig.Key}'");
                }

                var serviceType = GetType(baseConfig.ServiceType);
                var interfaceType = GetType(baseConfig.InterfaceType);
                var configType = GetType(baseConfig.ConfigurationType);

                sc.AddTransient(interfaceType, serviceType);
                var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
                    .GetMethods()
                    .First(m => m.Name == "Configure"
                                && m.GetParameters().Length == 2
                                && m.GetParameters()[1].ParameterType == typeof(IConfiguration))
                    .MakeGenericMethod(configType);

                configureMethod.Invoke(null, [sc, serviceConfig]);

                var jobKey = new JobKey(serviceConfig.Key, "PolledServices");

                q.AddJob(serviceType, jobKey, j => j
                    .WithDescription(baseConfig.Description));

                q.AddTrigger(t => t
                    .WithIdentity($"{serviceConfig.Key} Cron Trigger")
                    .ForJob(jobKey)
                    .StartNow()
                    .WithCronSchedule(baseConfig.CronSchedule));
            }
        });

        sc.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });

        return sc;
    }

    private static Type GetType(string typeName)
    {
        var serviceType = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(asm => asm.FullName?.StartsWith("Defra.Identity", StringComparison.OrdinalIgnoreCase) == true)
            .SelectMany(asm => asm.GetTypes())
            .FirstOrDefault(t => string.Equals(t.FullName, typeName, StringComparison.OrdinalIgnoreCase));

        return serviceType ?? throw new InvalidOperationException($"Type '{typeName}' not found");
    }
}
