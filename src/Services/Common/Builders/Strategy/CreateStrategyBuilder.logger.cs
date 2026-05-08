// <copyright file="CreateStrategyBuilder`2.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using Microsoft.Extensions.Logging;

public partial class CreateStrategyBuilder<TService, TEntity>
{
    [LoggerMessage(LogLevel.Information, "Executing {ActionDescription} {EntityDescription} with by operator {OperatorId}")]
    static partial void LogExecutingActionEntityWithByOperatorId(ILogger<TService> logger, string actionDescription, string entityDescription, Guid operatorId);

    [LoggerMessage(LogLevel.Information, "Successfully executed {ActionDescription} {EntityDescription} by operator {OperatorId}")]
    static partial void LogSuccessfullyExecutedActionEntityByOperatorId(ILogger<TService> logger, string actionDescription, string entityDescription, Guid operatorId);
}
