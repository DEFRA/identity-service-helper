// <copyright file="KeeperReferenceDataJob.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Scheduling;

using Microsoft.Extensions.Logging;

public partial class KeeperReferenceDataJob
{
    [LoggerMessage(LogLevel.Information, "{Job} starting {Date}")]
    partial void LogJobStartingDate(string job, DateTime date);

    [LoggerMessage(LogLevel.Information, "Fetching sites since {Date}")]
    partial void LogFetchingSitesSinceDate(DateTime date);

    [LoggerMessage(LogLevel.Information, "{Job} succeeded. Found {Count} sites.")]
    partial void LogJobSucceededFoundCountSites(string job, int count);

    [LoggerMessage(LogLevel.Warning, "{Job} cancelled.")]
    partial void LogJobCancelled(string job);

    [LoggerMessage(LogLevel.Error, "{Job} failed")]
    partial void LogJobFailed(string job, Exception exception);
}
