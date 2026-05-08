// <copyright file="CountyParishHoldingDelegationsNotificationsRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Messaging;

using Microsoft.Extensions.Logging;

public partial class CountyParishHoldingDelegationsNotificationsRepository
{
    [LoggerMessage(LogLevel.Information, "Getting single delegation notification")]
    partial void LogGettingSingleDelegationNotification();

    [LoggerMessage(LogLevel.Information, "Getting list of delegation notifications")]
    partial void LogGettingListOfDelegationNotifications();

    [LoggerMessage(LogLevel.Information, "Creating delegation notification for delegation {DelegationId} and message {MessageId}")]
    partial void LogCreatingDelegationNotificationForDelegationAndMessage(Guid delegationId, int messageId);

    [LoggerMessage(LogLevel.Information, "Updating delegation notification for delegation {DelegationId} and message {MessageId}")]
    partial void LogUpdatingDelegationNotificationForDelegationAndMessage(Guid delegationId, int messageId);

    [LoggerMessage(LogLevel.Information, "Deleting delegation notification with operator id {OperatorId}")]
    partial void LogDeletingDelegationNotificationWithOperatorId(Guid operatorId);

    [LoggerMessage(LogLevel.Warning, "Delegation notification not found for deletion")]
    partial void LogDelegationNotificationNotFoundForDeletion();
}
