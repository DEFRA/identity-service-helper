// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Ingest;

using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddDataIngestServices(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddTransient<IIngestDataService<CountyParishHoldings>, IngestCountyParishHoldings>();
        services.AddTransient<IIngestDataService<Roles>, IngestRoles>();
    }
}
