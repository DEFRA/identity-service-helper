// <copyright file="ReferenceRule.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Strategy.Rules.Models;

using Microsoft.Extensions.Logging;

public partial class ReferenceRule<TService, TEntity>
{
    [LoggerMessage(LogLevel.Warning,
        "Execute {ActionDescription} [{EntityDescription}] failed reference rule '{Description}'")]
    static partial void LogEntityReferenceNotFound(ILogger<TService> logger, string actionDescription,
        string entityDescription, string description);
}
