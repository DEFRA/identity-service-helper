// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData;

using System.Net;
using Defra.Identity.KeeperReferenceData.Configuration;
using Defra.Identity.KeeperReferenceData.Exceptions;
using Defra.Identity.KeeperReferenceData.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKeeperRecordsDataIntegrationService(this IServiceCollection services, IConfigurationRoot configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var krdsApi = configuration.GetRequiredSection(nameof(KrdsApi)).Get<KrdsApi>();
        if (krdsApi == null)
        {
            throw new KeeperReferenceDataConfigurationException($"Configuration section '{nameof(KrdsApi)}' not found.");
        }

        services.Configure<KrdsApi>(configuration.GetSection(nameof(KrdsApi)))
            .AddHttpClient<ISitesProvider, SitesProvider>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(krdsApi.Url);
            }).SetHandlerLifetime(TimeSpan.FromMinutes(5));

        return services;
    }
}
