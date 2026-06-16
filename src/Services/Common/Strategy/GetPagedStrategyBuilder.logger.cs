// <copyright file="GetPagedStrategyBuilder.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Strategy;

using Microsoft.Extensions.Logging;

public partial class GetPagedStrategyBuilder<TService, TEntity>
{
    [LoggerMessage(LogLevel.Information, "Executing {ActionDescription} [{EntityDescription}]")]
    static partial void LogExecutingAction(
        ILogger<TService> logger,
        string actionDescription,
        string entityDescription);

    [LoggerMessage(LogLevel.Information, "Successfully executed {ActionDescription} [{EntityDescription}]")]
    static partial void LogSuccessfullyExecutedAction(
        ILogger<TService> logger,
        string actionDescription,
        string entityDescription);
}
