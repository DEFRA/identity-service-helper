// <copyright file="IngestCountyParishHoldings.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Ingest.CountyParishHoldings;

using Defra.Identity.KeeperReferenceData.Models.Sites;
using Defra.Identity.KeeperReferenceData.Providers;

public class IngestService(IKrdsProvider provider, IDataService<Site> service) : IIngestService<Postgres.Database.Entities.CountyParishHoldings>
{
    private const string Cphcode = "CPHN";

    public async Task<bool> Execute()
    {
        var responses = await GetSites(DateTime.UtcNow);
        var sites = responses.Values.Where(t => (string)t.Type! == Cphcode).ToList();

        foreach (var cph in sites)
        {
           await service.Upsert(cph);
        }

        return true;
    }

    private async Task<SiteResponse> GetSites(DateTime since)
    {
        return await provider.Sites(since, CancellationToken.None);
    }
}
