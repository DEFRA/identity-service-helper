// <copyright file="MessagingFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Services;

using System.Net;
using Defra.Identity.Messaging.Models.Request;
using Defra.Identity.Messaging.Models.Response;
using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class MessagingFactory(
    ILogger<MessagingFactory> logger,
    IMessagingService messagingService,
    IExternalMessagingRepository externalMessagingRepository,
    ICountyParishHoldingDelegationsNotificationsRepository notificationRepository) : IMessagingFactory
{
    public async Task<MessageResponse> SendDelegationEmailAsync(DelegationEmailMessage request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Starting to send delegation email. CphDelegationId: {CphDelegationId}, Recipient: {Recipient}, TemplateId: {TemplateId}",
            request.CphDelegationId,
            request.Recipient,
            request.TemplateId);

        var message = await InternalQueueDelegationEmailAsync(request, cancellationToken);
        var result = await messagingService
            .SendEmailMessageAsync(request)
            .ConfigureAwait(false);
        message.ResponseCode = result.Status;
        message.NotifyId = Guid.Parse(result.NotifyId);
        message.SentAt = DateTime.UtcNow;
        message.ResponseMessage = result.IsSuccess ? null : JsonConvert.SerializeObject(result.Errors);

        await externalMessagingRepository.Update(message, cancellationToken);

        logger.LogInformation(
            "Delegation email send operation completed successfully. CphDelegationId: {CphDelegationId}, NotifyId: {NotifyId}",
            request.CphDelegationId,
            result.NotifyId);
        return result;
    }

    public async Task QueueDelegationEmailAsync(
        DelegationEmailMessage request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Pushing message to database for processing. CphDelegationId: {CphDelegationId}, Recipient: {Recipient}, TemplateId: {TemplateId}",
            request.CphDelegationId,
            request.Recipient,
            request.TemplateId);

        await InternalQueueDelegationEmailAsync(request, cancellationToken);
    }

    private async Task<ExternalMessaging> InternalQueueDelegationEmailAsync(DelegationEmailMessage request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Pushing message to database for processing. CphDelegationId: {CphDelegationId}, Recipient: {Recipient}, TemplateId: {TemplateId}", request.CphDelegationId, request.Recipient, request.TemplateId);

        var message = await externalMessagingRepository.Create(
            new ExternalMessaging()
            {
                MessageType = MessageTypes.Email,
                NotifyId = Guid.Empty,
                MessageRecipient = request.Recipient,
                TemplateId = request.TemplateId,
                RequestPayload = request.Payload.ToString(),
                ResponseCode = HttpStatusCode.Accepted,
                CreatedAt = DateTime.UtcNow,
            },
            cancellationToken);

        await notificationRepository.Create(
            new CountyParishHoldingDelegationsNotifications()
            {
                DelegationId = request.CphDelegationId, Message = message,
            },
            cancellationToken);

        return message;
    }
}
