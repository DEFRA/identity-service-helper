// <copyright file="ReferenceRulesBuilder.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>
namespace Defra.Identity.Services.Common.Builders.Rules;

using Microsoft.Extensions.Logging;

public partial class ReferenceRulesBuilder<TService>
{
    [LoggerMessage(LogLevel.Warning, "Execute {ActionDescription} {EntityDescription} failed reference rule '{Description}'")]
    static partial void LogExecuteActionEntityFailedReferenceRule(ILogger<TService> logger, string actionDescription, string entityDescription, string description);
}
