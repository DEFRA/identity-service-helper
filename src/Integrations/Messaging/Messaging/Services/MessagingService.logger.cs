// <copyright file="MessagingService.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Services;

using Defra.Identity.Postgres.Database;
using Microsoft.Extensions.Logging;
using Notify.Exceptions;

public partial class MessagingService
{
    [LoggerMessage(LogLevel.Information, "Get message notification. NotifyId: {NotificationId}")]
    partial void LogGetMessageNotificationNotifyId(string notificationId);

    [LoggerMessage(LogLevel.Information, "Get message notification. NotifyId: {NotifyId}, Status: {Status}")]
    partial void LogGetMessageNotificationNotifyIdStatus(string notifyId, string status);

    [LoggerMessage(LogLevel.Information, "Sending {MessageType} message. Recipient: {Recipient}, TemplateId: {TemplateId}")]
    partial void LogSendingMessageType(MessageTypes messageType, string recipient, Guid templateId);

    [LoggerMessage(LogLevel.Information, "{MessageType} sent. Recipient: {Recipient}, TemplateId: {TemplateId}, NotifyId: {NotifyId}")]
    partial void LogMessageTypeSent(MessageTypes messageType, string recipient, Guid templateId, string notifyId);

    [LoggerMessage(LogLevel.Error, "Error sending message. Recipient: {Recipient}, TemplateId: {TemplateId}, Reason: {Reason}")]
    partial void LogErrorSendingMessage(string recipient, Guid templateId, string reason, NotifyClientException notifyClientException);
}
