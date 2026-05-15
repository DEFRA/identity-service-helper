// <copyright file="CphDelegationsService.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations;

using Microsoft.Extensions.Logging;

public partial class CphDelegationsService
{
    [LoggerMessage(LogLevel.Warning, "Execute {ActionDescription} {EntityDescription} failed reference rule '{Description}'")]
    static partial void LogDelegatedUserNotFound(ILogger<CphDelegationsService> logger, string actionDescription, string entityDescription, string description);
}
