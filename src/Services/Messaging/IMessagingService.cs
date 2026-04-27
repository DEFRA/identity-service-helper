// <copyright file="IMessagingService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Messaging;

using Defra.Identity.Models.Requests.Messaging;
using Defra.Identity.Models.Responses.Messaging;

public interface IMessagingService
{
    MessageResponse SendEmailMessage(Message request);

    MessageResponse SendSmsMessage(Message message);

    Task<MessageStatus> GetNotificationAsync(string id);
}
