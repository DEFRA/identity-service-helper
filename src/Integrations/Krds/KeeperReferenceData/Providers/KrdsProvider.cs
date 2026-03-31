// <copyright file="SitesProvider.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

using System.Text.Json;
using System.Web;
using Defra.Identity.KeeperReferenceData.Models;
using Defra.Identity.KeeperReferenceData.Models.Locations;
using Defra.Identity.KeeperReferenceData.Models.Parties;
using Microsoft.Extensions.Logging;

public class KrdsProvider(HttpClient client, ILogger<KrdsProvider> logger) : IKrdsProvider
{
    public async Task<SiteResponse> Sites(DateTime since, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting sites");
        var request = string.Concat(client.BaseAddress, GetSitesSince(since));

        logger.LogInformation("Request: {Request}", request);
        using var response = await client.GetAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrEmpty(result))
        {
            return new SiteResponse();
        }

        try
        {
            if (result.TrimStart().StartsWith('['))
            {
                var sites = JsonSerializer.Deserialize<List<KeeperReferenceData.Models.Locations.Site>>(result);
                return new SiteResponse
                {
                    Values = sites ?? [],
                    Count = sites?.Count ?? 0,
                };
            }

            return JsonSerializer.Deserialize<SiteResponse>(result)!;
        }
        catch (JsonException jsonException)
        {
            logger.LogError(jsonException, "Error deserializing sites. Body: {ResponseBody}", result);
            throw;
        }
    }

    public async Task<PartyResponse> Parties(DateTime since, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting parties");
        var request = string.Concat(client.BaseAddress, GetPartiesSince(since));

        logger.LogInformation("Request: {Request}", request);
        using var response = await client.GetAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrEmpty(result))
        {
            return new PartyResponse();
        }

        try
        {
            if (result.TrimStart().StartsWith('['))
            {
                var parties = JsonSerializer.Deserialize<List<KeeperReferenceData.Models.Parties.Party>>(result);
                return new PartyResponse
                {
                    Values = parties ?? [],
                    Count = parties?.Count ?? 0,
                };
            }

            return JsonSerializer.Deserialize<PartyResponse>(result)!;
        }
        catch (JsonException jsonException)
        {
            logger.LogError(jsonException, "Error deserializing parties. Body: {ResponseBody}", result);
            throw;
        }
    }

    private static string GetSitesSince(DateTime since) => $"sites?since={HttpUtility.UrlEncode(since.ToString("yyyy-MM-dd"))}";

    private static string GetPartiesSince(DateTime since) => $"parties?since={HttpUtility.UrlEncode(since.ToString("yyyy-MM-dd"))}";

    public void Dispose()
    {
        client.Dispose();
    }
}
