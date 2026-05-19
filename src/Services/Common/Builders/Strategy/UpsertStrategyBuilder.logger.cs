// <copyright file="UpsertStrategyBuilder.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using Microsoft.Extensions.Logging;

public partial class UpsertStrategyBuilder<TService, TEntity>
{
    [LoggerMessage(LogLevel.Information, "Executing {ActionDescription} [{EntityDescription}] with id {Id} by operator {OperatorId}")]
    static partial void LogExecutingAction(ILogger<TService> logger, string actionDescription, string entityDescription, string id, Guid operatorId);

    [LoggerMessage(LogLevel.Information, "Successfully executed {ActionDescription} [{EntityDescription}] with id {Id} by operator {OperatorId}")]
    static partial void LogSuccessfullyExecutedAction(ILogger<TService> logger, string actionDescription, string entityDescription, string id, Guid operatorId);
}
