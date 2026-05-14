// <copyright file="KeeperReferenceDataJob.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Scheduling;

using Defra.Identity.KeeperReferenceData.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

[DisallowConcurrentExecution]
public partial class KeeperReferenceDataJob(
    IKrdsProvider krdsService,
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
            var sites = await krdsService.Sites(since, context.CancellationToken);

            LogJobSucceededFoundCountSites(context.JobDetail.Key.Name, sites.Count);
        }
        catch (OperationCanceledException)
        {
            LogJobCancelled(context.JobDetail.Key.Name);
            throw;
        }
        catch (Exception ex)
        {
            LogJobFailed(context.JobDetail.Key.Name, ex);
            throw;
        }
    }
}
