// <copyright file="IMessagingFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Messaging;

using Defra.Identity.Models.Requests.Delegations;

public interface IMessagingFactory
{
    Task SendDelegationEmailAsync(DelegationEmailMessage request, CancellationToken cancellationToken = default);
}
