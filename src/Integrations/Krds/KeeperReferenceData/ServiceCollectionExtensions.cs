// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData;

using Defra.Identity.KeeperReferenceData.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKeeperRecordsDataIntegrationService(this IServiceCollection services, IConfigurationRoot config)
    {
        var krdsApiConfig = config.GetSection(nameof(KrdsApi));
        services.AddSingleton(krdsApiConfig);

        return services;
    }
}
