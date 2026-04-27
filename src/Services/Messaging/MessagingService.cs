// <copyright file="MessagingService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Messaging;

using System.Net;
using System.Text.RegularExpressions;
using Defra.Identity.Models;
using Defra.Identity.Models.Requests.Messaging;
using Defra.Identity.Models.Responses.Messaging;
using Defra.Identity.Services.Configuration;
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
    public MessageResponse SendEmailMessage(Message request)
    {
        logger.LogInformation(
            "Sending email message. Recipient: {Recipient}, TemplateId: {TemplateId}",
            request.Recipient,
            request.TemplateId);

        var result = SendMessageAsync(SenderFunc, request);
        result.MessageType = MessageTypes.Email;

        logger.LogInformation(
            "Email sent. Recipient: {Recipient}, TemplateId: {TemplateId}, NotifyId: {NotifyId}",
            request.Recipient,
            request.TemplateId,
            result.NotifyId);
        return result;

        NotificationResponse SenderFunc(NotificationClient client) =>
            client.SendEmail(
                request.Recipient,
                request.TemplateId.ToString(),
                request.Payload);
    }

    public MessageResponse SendSmsMessage(Message request)
    {
        logger.LogInformation(
            "Sending SMS message. Recipient: {Recipient}, TemplateId: {TemplateId}",
            request.Recipient,
            request.TemplateId);

        var result = SendMessageAsync(SenderFunc, request);
        result.MessageType = MessageTypes.Sms;

        logger.LogInformation(
            "SMS sent. Recipient: {Recipient}, TemplateId: {TemplateId}, NotifyId: {NotifyId}",
            request.Recipient,
            request.TemplateId,
            result.NotifyId);
        return result;

        NotificationResponse SenderFunc(NotificationClient client) =>
            client.SendSms(
                request.Recipient,
                request.TemplateId.ToString(),
                request.Payload);
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

    private MessageResponse SendMessageAsync(Func<NotificationClient, NotificationResponse> senderFunc, Message request)
    {
        var client = new NotificationClient(config.Value.ApiKey);
        try
        {
            var response = senderFunc(client);

            return new MessageResponse()
            {
                NotifyId = response.id,
                Recipient = request.Recipient,
                TemplateId = request.TemplateId.ToString(),
                Status = HttpStatusCode.OK,
            };
        }
        catch (NotifyClientException ex)
        {
            var pattern = @"\{(?:[^{}]|(?<open>\{)|(?<-open>\}))*\}(?(open)(?!))";
            var match = Regex.Match(ex.Message, pattern);
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
}
