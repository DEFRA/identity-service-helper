using Livestock.Auth.Config;
using Livestock.Auth.Services;
using Livestock.Auth.Services.Config;
using Quartz;

namespace Livestock.Auth.Extensions;

public static class QuartzServiceExtensions
{
    public static IServiceCollection AddQuartzServices(this IServiceCollection sc, IConfigurationRoot config)
    {
        var polledServices = config.GetSection("PolledServices");
        Requires.NotNull(polledServices);

        sc.AddQuartz(q =>
        {
            foreach (var serviceConfig in polledServices.GetChildren())
            {
                var baseConfig = serviceConfig.Get<BasePollingServiceConfiguration>();
                Requires.NotNull(baseConfig);
        
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
                    .WithDescription(baseConfig.Description)
                );

                q.AddTrigger(t => t
                    .WithIdentity($"{serviceConfig.Key} Cron Trigger")
                    .ForJob(jobKey)
                    .StartNow()
                    .WithCronSchedule(baseConfig.CronSchedule)
                );
            }
        });

        sc.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
        
        return sc;
    }

    private static Type GetType(string typeName)
    {
        var serviceType = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(asm => asm.FullName?.StartsWith("Livestock.Auth", StringComparison.OrdinalIgnoreCase) == true)
            .SelectMany(asm => asm.GetTypes())
            .FirstOrDefault(t => string.Equals(t.FullName, typeName, StringComparison.OrdinalIgnoreCase));

        if (serviceType == null)
        {
            throw new InvalidOperationException($"Type '{typeName}' not found");
        }

        return serviceType;
    }
}