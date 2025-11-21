// <copyright file="AzureB2CSyncService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Polling.Services;

using Defra.Identity.Services.Polling.Config;
using Defra.Identity.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

public class AzureB2CSyncService(
    ILogger<AzureB2CSyncService> logger,
    IOptions<AzureB2CSyncServiceConfiguration> options)
    : IAzureB2CSyncService
{
    public async Task Execute(IJobExecutionContext context)
    {
        var config = options.Value;
        using var stopwatch = new StopwatchLogger(logger);

        await Task.Delay(TimeSpan.FromSeconds(5), context.CancellationToken);
    }
}
