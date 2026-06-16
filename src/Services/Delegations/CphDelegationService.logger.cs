// <copyright file="CphDelegationService.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations;

using Microsoft.Extensions.Logging;

public partial class CphDelegationService
{
    [LoggerMessage(LogLevel.Warning,
        "Execute {ActionDescription} [{EntityDescription}] failed reference rule '{Description}'")]
    static partial void LogDelegatedUserNotFound(ILogger<CphDelegationService> logger, string actionDescription,
        string entityDescription, string description);

    [LoggerMessage(LogLevel.Warning,
        "Delegation accept or reject invitation failed authorisation rules for delegation with id {delegationId} and operator {operatorId}")]
    static partial void LogAcceptOrRejectInvitationFailedAuthorisationRules(
        ILogger<CphDelegationService> logger,
        Guid delegationId,
        Guid operatorId);
}
