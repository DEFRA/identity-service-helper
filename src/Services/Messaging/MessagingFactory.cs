// <copyright file="MessagingFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Messaging;

using System.Net;
using Defra.Identity.Models.Requests.Delegations;
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
    public async Task SendDelegationEmailAsync(DelegationEmailMessage request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting to send delegation email. CphDelegationId: {CphDelegationId}, Recipient: {Recipient}, TemplateId: {TemplateId}", request.CphDelegationId, request.Recipient, request.TemplateId);

        var message = await externalMessagingRepository.Create(
            new ExternalMessaging()
            {
                MessageType = request.Type,
                NotifyId = Guid.Empty,
                MessageRecipient = request.Recipient,
                TemplateId = request.TemplateId,
                RequestPayload = request.Payload.ToString(),
                ResponseCode = (int)HttpStatusCode.NoContent,
                CreatedAt = DateTime.UtcNow,
            },
            cancellationToken);

        var result = messagingService.SendEmailMessage(request);
        message.NotifyId = Guid.Parse(result.NotifyId);

        if (!result.IsSuccess)
        {
            message.ResponseCode = (int)result.Status;
            message.ResponseMessage = JsonConvert.SerializeObject(result.Errors);
        }

        await externalMessagingRepository.Update(message, cancellationToken);
        await notificationRepository.Create(
            new CountyParishHoldingDelegationsNotifications()
            {
                DelegationId = request.CphDelegationId, Message = message,
            },
            cancellationToken);

        logger.LogInformation("Delegation email send operation completed successfully. CphDelegationId: {CphDelegationId}, NotifyId: {NotifyId}", request.CphDelegationId, result.NotifyId);
    }
}
