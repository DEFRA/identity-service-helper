// <copyright file="UpsertStrategyBuilder.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Strategy;

using Microsoft.Extensions.Logging;

public partial class UpsertStrategyBuilder<TService, TEntity>
{
    [LoggerMessage(LogLevel.Information, "Executing {ActionDescription} [{EntityDescription}] by operator {OperatorId}")]
    static partial void LogExecutingAction(ILogger<TService> logger, string actionDescription, string entityDescription, Guid operatorId);

    [LoggerMessage(LogLevel.Information, "Successfully executed {ActionDescription} [{EntityDescription}] by operator {OperatorId}")]
    static partial void LogSuccessfullyExecutedAction(ILogger<TService> logger, string actionDescription, string entityDescription, Guid operatorId);
}
