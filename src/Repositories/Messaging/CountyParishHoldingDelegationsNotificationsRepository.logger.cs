// <copyright file="CountyParishHoldingDelegationsNotificationsRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Messaging;

using Microsoft.Extensions.Logging;

public partial class CountyParishHoldingDelegationsNotificationsRepository
{
    [LoggerMessage(LogLevel.Information, "Creating delegation notification for delegation {DelegationId} and message {MessageId}")]
    partial void LogCreatingDelegationNotificationForDelegationAndMessage(Guid delegationId, int messageId);
}
