// <copyright file="SitesProvider.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

using Microsoft.Extensions.Logging;

public partial class SitesProvider
{
    [LoggerMessage(LogLevel.Information, "Getting sites")]
    partial void LogGettingSites();

    [LoggerMessage(LogLevel.Information, "Request: {Request}")]
    partial void LogRequestRequest(string request);
}
