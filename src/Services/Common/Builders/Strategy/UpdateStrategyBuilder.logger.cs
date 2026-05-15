// <copyright file="UpdateStrategyBuilder.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using Microsoft.Extensions.Logging;

public partial class UpdateStrategyBuilder<TService, TEntity>
{
    [LoggerMessage(LogLevel.Information, "Executing {ActionDescription} [{EntityDescription}] with id {Id} by operator {OperatorId}")]
    static partial void LogExecutingAction(ILogger<TService> logger, string actionDescription, string entityDescription, string id, Guid operatorId);

    [LoggerMessage(LogLevel.Warning, "{EntityDescription} with id {Id} not found")]
    static partial void LogEntityWithIdNotFound(ILogger<TService> logger, string entityDescription, string id);

    [LoggerMessage(LogLevel.Information, "Successfully executed {ActionDescription} [{EntityDescription}] with id {Id} by operator {OperatorId}")]
    static partial void LogSuccessfullyExecutedAction(ILogger<TService> logger, string actionDescription, string entityDescription, string id, Guid operatorId);
}
