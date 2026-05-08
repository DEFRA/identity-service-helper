// <copyright file="MessagingFactory.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Services;

using Microsoft.Extensions.Logging;

public partial class MessagingFactory
{
    [LoggerMessage(LogLevel.Information, "Starting to send delegation email. CphDelegationId: {CphDelegationId}, Recipient: {Recipient}, TemplateId: {TemplateId}")]
    partial void LogStartingToSendDelegationEmail(Guid cphDelegationId, string recipient, Guid templateId);

    [LoggerMessage(LogLevel.Information, "Delegation email send operation completed successfully. CphDelegationId: {CphDelegationId}, NotifyId: {NotifyId}")]
    partial void LogDelegationEmailSendOperationCompleted(Guid cphDelegationId, string notifyId);

    [LoggerMessage(LogLevel.Information, "Pushing message to database for processing. CphDelegationId: {CphDelegationId}, Recipient: {Recipient}, TemplateId: {TemplateId}")]
    partial void LogPushingMessageToDatabaseForProcessing(Guid cphDelegationId, string recipient, Guid templateId);
}
