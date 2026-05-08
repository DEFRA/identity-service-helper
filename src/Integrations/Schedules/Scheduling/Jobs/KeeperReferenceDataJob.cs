// <copyright file="KeeperReferenceDataJob.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Scheduling.Jobs;

using Defra.Identity.KeeperReferenceData.Providers;
using Microsoft.Extensions.Logging;
using Quartz;

/// <summary>
/// KRDS data sync job entry point.
/// </summary>
/// <param name="sitesService">Instance of the site service.</param>
/// <param name="logger">Instance of the logger service.</param>
[DisallowConcurrentExecution]
public partial class KeeperReferenceDataJob(
    ISitesProvider sitesService,
    ILogger<KeeperReferenceDataJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            LogJobStartingDate(context.JobDetail.Key.Name, DateTime.UtcNow);

            // We fetch since 24 hours ago as a default, or we could add it to options
            var since = DateTime.UtcNow.AddDays(-1);
            LogFetchingSitesSinceDate(since);
            var sites = await sitesService.Sites(since, context.CancellationToken);

            LogJobSucceededFoundCountSites(context.JobDetail.Key.Name, sites.Count);
        }
        catch (OperationCanceledException ex)
        {
            LogJobCancelled(context.JobDetail.Key.Name, ex);
        }
        catch (Exception ex)
        {
            LogJobFailed(context.JobDetail.Key.Name, ex);
        }
    }
}
