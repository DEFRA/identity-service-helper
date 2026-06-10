// <copyright file="CphDelegationsService.InvitationResponse.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations;

using Microsoft.Extensions.Logging;

public partial class CphDelegationsService
{
    [LoggerMessage(
        EventId = 60,
        Level = LogLevel.Warning,
        Message = "Invitation token validation failed for delegation with id {Id}")]
    private static partial void LogInvitationTokenValidationFailed(ILogger logger, string id);

    [LoggerMessage(
        EventId = 61,
        Level = LogLevel.Warning,
        Message = "Invitation response for delegation with id {Id} failed business rule '{Description}'")]
    private static partial void LogInvitationResponseFailedBusinessRule(ILogger logger, string id, string description);

    [LoggerMessage(
        EventId = 62,
        Level = LogLevel.Warning,
        Message = "Invitation response delegation with id {Id} not found")]
    private static partial void LogInvitationDelegationNotFound(ILogger logger, string id);
}
