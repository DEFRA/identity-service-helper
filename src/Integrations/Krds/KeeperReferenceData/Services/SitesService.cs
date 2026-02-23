// <copyright file="SitesService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Services;

using Defra.Identity.KeeperReferenceData.Configuration;
using Defra.Identity.KeeperReferenceData.Models;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class SitesService(ILogger<SitesService> logger, IOptions<KrdsApi> api) : ISitesService
{
    public async Task<List<Site>> Sites(DateTime since, CancellationToken cancellationToken)
    {
        var url = api.Value.Url
            .WithHeader("x-api-key", api.Value.Key)
            .AppendPathSegment("sites")
            .SetQueryParam("since", since.ToString("yyyy-MM-ddTHH:mm:ssZ"));

        logger.LogInformation("Calling KRDS API: {url}", url);

        try
        {
            var result = await url.GetJsonAsync<Site[]>(cancellationToken: cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Found {count} Sites from the API", result.Length);
            return result.ToList();
        }
        catch (FlurlHttpException ex)
        {
            logger.LogError(ex, "Error calling KRDS API: {url}", url);
            throw;
        }
    }
}
