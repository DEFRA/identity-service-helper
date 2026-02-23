// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData;

using Defra.Identity.KeeperReferenceData.Configuration;
using Defra.Identity.KeeperReferenceData.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKeeperRecordsDataIntegrationService(this IServiceCollection services, IConfigurationRoot configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var krdsApiSection = configuration.GetRequiredSection(nameof(KrdsApi));
        if (krdsApiSection.Exists() == false)
        {
            throw new Exception($"Configuration section '{nameof(KrdsApi)}' not found.");
        }

        services.AddSingleton<ISitesService, SitesService>();

        return services;
    }
}
