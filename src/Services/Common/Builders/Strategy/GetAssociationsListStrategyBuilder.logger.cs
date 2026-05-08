// <copyright file="GetAssociationsListStrategyBuilder.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using Microsoft.Extensions.Logging;

public partial class GetAssociationsListStrategyBuilder<TService, TPrimary, TAssociation>
{
    [LoggerMessage(LogLevel.Information, "Executing {ActionDescription} {EntityDescription} with id {Id}")]
    static partial void LogExecutingActionEntityWithId(
        ILogger<TService> logger,
        string actionDescription, string entityDescription, Guid id);

    [LoggerMessage(LogLevel.Warning, "{EntityDescription} with id {Id} not found")]
    static partial void LogEntityWithIdNotFound(
        ILogger<TService> logger,
        string entityDescription,
        Guid id);

    [LoggerMessage(LogLevel.Information, "Successfully executed {ActionDescription} {EntityDescription} with id {Id}")]
    static partial void LogSuccessfullyExecutedActionEntityWithId(
        ILogger<TService> logger,
        string actionDescription,
        string entityDescription,
        Guid id);
}
