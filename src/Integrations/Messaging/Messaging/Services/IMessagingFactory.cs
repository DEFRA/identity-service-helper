// <copyright file="IMessagingFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Services;

using Defra.Identity.Messaging.Models.Request;
using Defra.Identity.Messaging.Models.Response;

public interface IMessagingFactory
{
    Task<MessageResponse> SendDelegationEmailAsync(
        DelegationEmailMessage request,
        CancellationToken cancellationToken = default);

    Task QueueDelegationEmailAsync(
        DelegationEmailMessage request,
        CancellationToken cancellationToken = default);
}
