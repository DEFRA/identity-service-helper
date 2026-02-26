// <copyright file="KeeperReferenceDataJob.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Scheduling;

using Defra.Identity.KeeperReferenceData.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

public class KeeperReferenceDataJob(
    ISitesProvider sitesService,
    ILogger<KeeperReferenceDataJob> logger,
    IOptions<KeeperReferenceDataOptions> options)
    : IJob
{
    private readonly KeeperReferenceDataOptions options = options.Value;

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation("KeeperReferenceDataJob starting");

            // For now, we fetch since 24 hours ago as a default, or we could add it to options
            var since = DateTime.UtcNow.AddDays(-1);
            logger.LogInformation("Fetching sites since {Date}", since);
            var sites = await sitesService.Sites(since, context.CancellationToken);

            logger.LogInformation("KeeperReferenceDataJob succeeded. Found {Count} sites.", sites.Count);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("KeeperReferenceDataJob cancelled.");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "KeeperReferenceDataJob failed");
            throw;
        }
    }
}
