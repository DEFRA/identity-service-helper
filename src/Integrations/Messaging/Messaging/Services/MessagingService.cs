// <copyright file="MessagingService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Services;

using System.Net;
using System.Text.RegularExpressions;
using Defra.Identity.Messaging.Configuration;
using Defra.Identity.Messaging.Models.Request;
using Defra.Identity.Messaging.Models.Response;
using Defra.Identity.Postgres.Database;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Notify.Client;
using Notify.Exceptions;
using Notify.Models.Responses;

public class MessagingService(
    ILogger<MessagingService> logger,
    IOptions<MessagingOptions> config) : IMessagingService
{
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);

    public async Task<MessageResponse> SendEmailMessageAsync(Message request)
    {
        return await SendMessageAsync(request, MessageTypes.Email)
            .ConfigureAwait(false);
    }

    public async Task<MessageResponse> SendSmsMessageAsync(Message request)
    {
        return await SendMessageAsync(request, MessageTypes.Sms)
            .ConfigureAwait(false);
    }

    public async Task<MessageStatus> GetNotificationAsync(string id)
    {
        logger.LogInformation("Get message notification. NotifyId: {NotificationId}", id);
        var client = new NotificationClient(config.Value.ApiKey);
        var notification = await client.GetNotificationByIdAsync(id);

        var result = new MessageStatus()
        {
            NotifyId = notification.id,
            Recipient = notification.emailAddress ?? notification.phoneNumber,
            MessageType = notification.type,
            TemplateId = notification.template?.id ?? string.Empty,
            Status = notification.status,
        };

        logger.LogInformation(
            "Get message notification. NotifyId: {NotifyId}, Status: {Status}",
            result.NotifyId,
            result.Status);
        return result;
    }

    private async Task<MessageResponse> SendMessageAsync(Message request, MessageTypes messageType)
    {
        logger.LogInformation(
            "Sending {MessageType} message. Recipient: {Recipient}, TemplateId: {TemplateId}",
            messageType,
            request.Recipient,
            request.TemplateId);

        var client = new NotificationClient(config.Value.ApiKey);
        try
        {
            var response = await InvokeSendAsync(request, client, messageType);

            var result = new MessageResponse()
            {
                NotifyId = response.id,
                Recipient = request.Recipient,
                MessageType = messageType,
                TemplateId = request.TemplateId.ToString(),
                Status = HttpStatusCode.OK,
            };

            logger.LogInformation(
                "{MessageType} sent. Recipient: {Recipient}, TemplateId: {TemplateId}, NotifyId: {NotifyId}",
                messageType,
                request.Recipient,
                request.TemplateId,
                result.NotifyId);
            return result;
        }
        catch (NotifyClientException ex)
        {
           return HandleNotifyClientException(ex, request);
        }
    }

    private async Task<NotificationResponse> InvokeSendAsync(
        Message request,
        NotificationClient client,
        MessageTypes messageType)
    {
        // Not an ideal way to do this, but using a generic type for the response results in covariance issues as
        // I need the base class for the response.
        return messageType switch
        {
            MessageTypes.Email => await client
                .SendEmailAsync(request.Recipient, request.TemplateId.ToString(), request.Payload)
                .ConfigureAwait(false),
            MessageTypes.Sms => await client
                .SendSmsAsync(request.Recipient, request.TemplateId.ToString(), request.Payload)
                .ConfigureAwait(false),
            _ => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null),
        };
    }

    private MessageResponse HandleNotifyClientException(NotifyClientException ex, Message request)
    {
        var pattern = @"\{(?:[^{}]|(?<open>\{)|(?<-open>\}))*\}(?(open)(?!))";
        var match = Regex.Match(ex.Message, pattern, RegexOptions.None, RegexTimeout);
        var result = new MessageResponse()
        {
            NotifyId = Guid.Empty.ToString(),
            Status = HttpStatusCode.InternalServerError,
            Errors =
            [
                new ErrorItem { Error = "An error has occurred", Message = ex.Message, }
            ],
        };

        if (match.Success)
        {
            var tmp = JsonConvert.DeserializeObject<ErrorResponse>(match.Value);
            result.Status = tmp.StatusCode;
            result.Errors = tmp.Errors;
        }

        logger.LogError(
            ex,
            "Error sending message. Recipient: {Recipient}, TemplateId: {TemplateId}, Reason: {Reason}",
            request.Recipient,
            request.TemplateId,
            result.Errors.First().Message);

        return result;
    }
}
