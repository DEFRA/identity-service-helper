// <copyright file="MessageQueueProcessor.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging;

using System.Linq.Expressions;
using System.Net;
using Defra.Identity.Messaging.Models;
using Defra.Identity.Messaging.Models.Request;
using Defra.Identity.Messaging.Models.Response;
using Defra.Identity.Messaging.Services;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class MessageQueueProcessor(
    IExternalMessagingRepository externalMessagingRepository,
    IMessagingService messagingService,
    ILogger<MessageQueueProcessor> logger)
    : IMessageQueueProcessor
{
    public async Task<ProcessResult> ProcessMessageQueueAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Processing message queue.");

        Expression<Func<ExternalMessaging, bool>> filter = x =>
            x.ResponseCode == HttpStatusCode.Accepted ||
            x.ResponseCode == HttpStatusCode.TooManyRequests;
        var messages = await externalMessagingRepository
            .GetList(filter, cancellationToken)
            .ConfigureAwait(false);

        if (messages.Count == 0)
        {
            logger.LogInformation("No messages to process.");
            return new ProcessResult();
        }

        logger.LogInformation(
            "{EmailCount} Emails and {SmsCount} Texts to process",
            messages.Count(x => x.MessageType == MessageTypes.Email),
            messages.Count(x => x.MessageType == MessageTypes.Sms));

        var result = await SendAndUpdateMessageAsync(messages, cancellationToken)
            .ConfigureAwait(false);

        logger.LogInformation(
            "Processed Email {SuccessEmailCount}-S, {ErrorEmailCount}-F, Text {SuccessSmsCount}-S, {ErrorSmsCount}-F",
            result.Success.EmailCountProcessed,
            result.Error.EmailCountProcessed,
            result.Success.SmsCountProcessed,
            result.Error.SmsCountProcessed);

        return result;
    }

    private async Task<ProcessResult> SendAndUpdateMessageAsync(List<ExternalMessaging> messages, CancellationToken cancellationToken)
    {
        var result = new ProcessResult();

        foreach (var message in messages)
        {
            var externalMessage = message.ToMessage();

            var sendResult = message.MessageType switch
            {
                MessageTypes.Email => await messagingService
                    .SendEmailMessageAsync(externalMessage)
                    .ConfigureAwait(false),
                MessageTypes.Sms => await messagingService
                    .SendSmsMessageAsync(externalMessage)
                    .ConfigureAwait(false),
                _ => throw new InvalidOperationException($"Unsupported message type: {message.MessageType}"),
            };

            message.ResponseCode = sendResult.Status;
            message.NotifyId = Guid.Parse(sendResult.NotifyId);
            message.SentAt = DateTime.UtcNow;

            var resultGroup = sendResult.IsSuccess ? result.Success : result.Error;
            message.ResponseMessage = sendResult.IsSuccess ? null : JsonConvert.SerializeObject(sendResult.Errors);
            if (message.MessageType == MessageTypes.Email)
            {
                resultGroup.EmailCountProcessed++;
            }
            else
            {
                resultGroup.SmsCountProcessed++;
            }

            await externalMessagingRepository
                .Update(message, cancellationToken)
                .ConfigureAwait(false);
        }

        return result;
    }
}
