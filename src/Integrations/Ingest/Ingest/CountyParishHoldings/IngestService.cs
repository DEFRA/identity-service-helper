// <copyright file="IngestCountyParishHoldings.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using Defra.Identity.KeeperReferenceData.Models.Sites;
using Defra.Identity.KeeperReferenceData.Providers;
using Defra.Identity.Services.Cphs;

namespace Defra.Identity.Ingest.CountyParishHoldings;

public class IngestService(IKrdsProvider provider, ICphService service) : IIngestService<Postgres.Database.Entities.CountyParishHoldings>
{
    private const string Cphcode = "CPHN";

    public async Task<bool> Execute()
    {
        var sites = await GetSites(DateTime.UtcNow);
        var cph = sites.Values.SelectMany(x => x.Identifiers).Where(t => t.Type is { Code: Cphcode }).ToList();

        foreach (var cphIdentifier in cph)
        {
           await service.Upsert(new Postgres.Database.Entities.CountyParishHoldings() { Identifier = cphIdentifier.Value });
        }

        return true;
    }

    private async Task<SiteResponse> GetSites(DateTime since)
    {
        return await provider.Sites(since, CancellationToken.None);
    }
}
