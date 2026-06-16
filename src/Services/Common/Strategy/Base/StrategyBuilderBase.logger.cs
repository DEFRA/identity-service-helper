// <copyright file="StrategyBuilderBase.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Strategy.Base;

using Microsoft.Extensions.Logging;

public abstract partial class StrategyBuilderBase<TService, TBuilder>
{
    [LoggerMessage(LogLevel.Warning, "Execute {ActionDescription} [{EntityDescription}] failed validation")]
    static partial void LogExecuteActionEntityFailedValidation(ILogger<TService> logger, string actionDescription, string entityDescription);
}
