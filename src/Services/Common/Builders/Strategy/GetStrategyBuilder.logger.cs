// <copyright file="GetStrategyBuilder.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using Microsoft.Extensions.Logging;

public partial class GetStrategyBuilder<TService, TEntity>
{
    [LoggerMessage(LogLevel.Information, "Executing {ActionDescription} {EntityDescription} with id {Id}")]
    static partial void LogExecutingActiondescriptionEntitydescriptionWithIdId(
        ILogger<TService> logger,
        string actionDescription,
        string entityDescription, Guid id);

    [LoggerMessage(LogLevel.Warning, "{EntityDescription} with id {Id} not found")]
    static partial void LogEntitydescriptionWithIdIdNotFound(
        ILogger<TService> logger,
        string entityDescription,
        Guid id);

    [LoggerMessage(LogLevel.Information, "Successfully executed {ActionDescription} {EntityDescription} with id {Id}")]
    static partial void LogSuccessfullyExecutedActiondescriptionEntitydescriptionWithIdId(
        ILogger<TService> logger,
        string actionDescription,
        string entityDescription,
        Guid id);
}
