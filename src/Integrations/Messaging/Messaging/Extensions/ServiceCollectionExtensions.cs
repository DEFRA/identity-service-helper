// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Extensions;

using Defra.Identity.Messaging.Configuration;
using Defra.Identity.Messaging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessagingIntegrationService(this IServiceCollection services, IConfigurationRoot configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<MessagingOptions>().BindConfiguration("Email");
        services.AddTransient<IMessagingFactory, MessagingFactory>();
        services.AddTransient<IMessagingService, MessagingService>();
        services.AddTransient<IMessageQueueProcessor, MessageQueueProcessor>();

        return services;
    }
}
