// <copyright file="GetListStrategyBuilder.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using Microsoft.Extensions.Logging;

public partial class GetListStrategyBuilder<TService, TEntity>
{
    [LoggerMessage(LogLevel.Information, "Executing {ActionDescription} {EntityDescription}")]
    static partial void LogExecutingActionEntity(
        ILogger<TService> logger,
        string actionDescription,
        string entityDescription);

    [LoggerMessage(LogLevel.Information, "Successfully executed {ActionDescription} {EntityDescription}")]
    static partial void LogSuccessfullyExecutedActionEntity(
        ILogger<TService> logger,
        string actionDescription,
        string entityDescription);
}
