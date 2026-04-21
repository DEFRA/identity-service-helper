// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using Defra.Identity.Ingest.CountyParishHoldings;
using Defra.Identity.Ingest.Roles;
using IngestService = Defra.Identity.Ingest.Roles.IngestService;

namespace Defra.Identity.Ingest;

using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddDataIngestServices(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddTransient<IIngestService<Postgres.Database.Entities.CountyParishHoldings>, CountyParishHoldings.IngestService>();
        services.AddTransient<IIngestService<Postgres.Database.Entities.Roles>, IngestService>();
    }
}
