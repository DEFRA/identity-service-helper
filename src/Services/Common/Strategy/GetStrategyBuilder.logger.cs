// <copyright file="GetStrategyBuilder.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Strategy;

using Microsoft.Extensions.Logging;

public partial class GetStrategyBuilder<TService, TEntity>
{
    [LoggerMessage(LogLevel.Information, "Executing {ActionDescription} [{EntityDescription}] with id {Id}")]
    static partial void LogExecutingAction(
        ILogger<TService> logger,
        string actionDescription,
        string entityDescription,
        string id);

    [LoggerMessage(LogLevel.Warning, "{EntityDescription} with id {Id} not found")]
    static partial void LogEntityWithIdNotFound(
        ILogger<TService> logger,
        string entityDescription,
        string id);

    [LoggerMessage(LogLevel.Information, "Successfully executed {ActionDescription} [{EntityDescription}] with id {Id}")]
    static partial void LogSuccessfullyExecutedAction(
        ILogger<TService> logger,
        string actionDescription,
        string entityDescription,
        string id);
}
