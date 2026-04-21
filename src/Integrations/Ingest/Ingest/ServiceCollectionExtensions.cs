// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Ingest;

using Defra.Identity.Ingest.CountyParishHoldings;
using Defra.Identity.Ingest.Roles;
using Defra.Identity.KeeperReferenceData.Models.Sites;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IngestService = Defra.Identity.Ingest.Roles.IngestService;

public static class ServiceCollectionExtensions
{
    public static void AddDataIngestServices(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddTransient<IDataService<Identifier>, CountyParishHoldings.DataService>();
        services.AddTransient<IDataService<Defra.Identity.KeeperReferenceData.Models.Parties.Role>, Roles.DataService>();
        services.AddTransient<IIngestService<Postgres.Database.Entities.CountyParishHoldings>, CountyParishHoldings.IngestService>();
        services.AddTransient<IIngestService<Postgres.Database.Entities.Roles>, IngestService>();
    }
}
