// <copyright file="IMessagingService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Services;

using Defra.Identity.Messaging.Models.Request;
using Defra.Identity.Messaging.Models.Response;

public interface IMessagingService
{
    Task<MessageResponse> SendEmailMessageAsync(Message request);

    Task<MessageResponse> SendSmsMessageAsync(Message request);

    Task<MessageStatus> GetNotificationAsync(string id);
}
