// <copyright file="Class1.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Ingest;

using Defra.Identity.KeeperReferenceData.Providers;
using Defra.Identity.Models.Integration.Krds.Locations;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Cphs;

public class IngestCountyParishHoldings(IKrdsProvider provider, ICphService service) : IIngestDataService<CountyParishHoldings>
{
    private const string Cphcode = "CPHN";

    public async Task<bool> Execute()
    {
        var sites = await GetSites(DateTime.UtcNow);
        var cph = sites.Values.SelectMany(x => x.Identifiers).Where(t => t.Type is { Code: Cphcode }).ToList();

        foreach (var cphIdentifier in cph)
        {
           await service.Upsert(new CountyParishHoldings() { Identifier = cphIdentifier.Value });
        }

        return true;
    }

    private async Task<SiteResponse> GetSites(DateTime since)
    {
        return await provider.Sites(since, CancellationToken.None);
    }
}
