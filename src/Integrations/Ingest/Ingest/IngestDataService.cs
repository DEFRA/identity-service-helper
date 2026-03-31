// <copyright file="Class1.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Ingest;

using Defra.Identity.KeeperReferenceData.Models.Locations;
using Defra.Identity.KeeperReferenceData.Models.Parties;
using Defra.Identity.KeeperReferenceData.Providers;
using Defra.Identity.Services.Cphs;

public class IngestDataService(IKrdsProvider provider) : IIngestDataService
{
    private const string Cphcode = "CPHN";

    public async Task<bool> Execute()
    {
        var sites = await GetSites(DateTime.UtcNow);
        var cph = sites.Values.SelectMany(x => x.Identifiers).Where(t => t.Type is { Code: Cphcode }).ToList();

        return true;
    }

    private async Task<SiteResponse> GetSites(DateTime since)
    {
        return await provider.Sites(since, CancellationToken.None);
    }
}
