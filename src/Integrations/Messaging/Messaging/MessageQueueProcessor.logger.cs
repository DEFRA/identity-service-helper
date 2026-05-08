// <copyright file="MessageQueueProcessor.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging;

using Microsoft.Extensions.Logging;

public partial class MessageQueueProcessor
{
    [LoggerMessage(LogLevel.Information, "Processing message queue.")]
    partial void LogProcessingMessageQueue();

    [LoggerMessage(LogLevel.Information, "No messages to process.")]
    partial void LogNoMessagesToProcess();

    [LoggerMessage(LogLevel.Information, "{EmailCount} Emails and {SmsCount} Texts to process")]
    partial void LogEmailCountAndSmsCount(int emailCount, int smsCount);

    [LoggerMessage(LogLevel.Information, "Processed Email {SuccessEmailCount}-S, {ErrorEmailCount}-F, Text {SuccessSmsCount}-S, {ErrorSmsCount}-F")]
    partial void LogProcessedEmailCounts(int successEmailCount, int errorEmailCount, int successSmsCount, int errorSmsCount);
}
