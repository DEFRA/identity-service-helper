// <copyright file="UpdateStrategyBuilder.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using Microsoft.Extensions.Logging;

public partial class UpdateStrategyBuilder<TService, TEntity>
{
    [LoggerMessage(LogLevel.Information, "Executing {ActionDescription} {EntityDescription} with id {Id} by operator {OperatorId}")]
    static partial void LogExecutingActionEntityWithIdByOperatorid(ILogger<TService> logger, string actionDescription, string entityDescription, Guid id, Guid operatorId);

    [LoggerMessage(LogLevel.Warning, "{EntityDescription} with id {Id} not found")]
    static partial void LogEntityWithIdNotFound(ILogger<TService> logger, string entityDescription, Guid id);

    [LoggerMessage(LogLevel.Information, "Successfully executed {ActionDescription} {EntityDescription} with id {Id} by operator {OperatorId}")]
    static partial void LogSuccessfullyExecutedActionEntityWithIdByOperatorid(ILogger<TService> logger, string actionDescription, string entityDescription, Guid id, Guid operatorId);
}
