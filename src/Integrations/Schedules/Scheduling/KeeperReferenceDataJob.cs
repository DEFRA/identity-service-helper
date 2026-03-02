// <copyright file="KeeperReferenceDataJob.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Scheduling;

using Defra.Identity.KeeperReferenceData.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

[DisallowConcurrentExecution]
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
            logger.LogInformation("{Job} starting {Date}", context.JobDetail.Key.Name, DateTime.UtcNow);

            // We fetch since 24 hours ago as a default, or we could add it to options
            var since = DateTime.UtcNow.AddDays(-1);
            logger.LogInformation("Fetching sites since {Date}", since);
            var sites = await sitesService.Sites(since, context.CancellationToken);

            logger.LogInformation("{Job} succeeded. Found {Count} sites.", context.JobDetail.Key.Name, sites.Count);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("{Job} cancelled.", context.JobDetail.Key.Name);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Job} failed", context.JobDetail.Key.Name);
            throw;
        }
    }
}
