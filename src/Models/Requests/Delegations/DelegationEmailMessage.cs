// <copyright file="DelegationEmailMessage.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Delegations;

using Defra.Identity.Models.Requests.Messaging;

public class DelegationEmailMessage(Guid cphDelegationId, MessageTemplateTypes.Delegation template) : Message
(
    MessageTypes.Email,
    template.Value)
{
    public Guid CphDelegationId { get; } = cphDelegationId;
}
