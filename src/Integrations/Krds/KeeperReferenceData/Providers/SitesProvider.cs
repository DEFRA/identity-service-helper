// <copyright file="SitesProvider.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

using System.Text.Json;
using Defra.Identity.KeeperReferenceData.Models;
using Microsoft.Extensions.Logging;

public class SitesProvider(HttpClient client, ILogger<SitesProvider> logger) : ISitesProvider
{
    public async Task<List<Site>> Sites(DateTime since, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting sites");
        var request = string.Concat(client.BaseAddress, GetSitesUrl);

        using var response = await client.GetAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        return string.IsNullOrEmpty(result) ? new List<Site>() : JsonSerializer.Deserialize<List<Site>>(result)!;
    }

    private static string GetSitesUrl => $"sites";

    public void Dispose()
    {
        client.Dispose();
    }
}
