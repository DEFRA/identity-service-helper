// <copyright file="DelegationEmailMessage.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Models.Request;

using Defra.Identity.Postgres.Database;

public class DelegationEmailMessage(Guid cphDelegationId, MessageTemplateTypes.Delegation template) : Message
(
    MessageTypes.Email,
    template.Value)
{
    public Guid CphDelegationId { get; } = cphDelegationId;
}
