// <copyright file="DelegationEmailMessageExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Models;

using Defra.Identity.Messaging.Models.Request;
using Defra.Identity.Postgres.Database.Entities;

public static class DelegationEmailMessageExtensions
{
    public static ExternalMessaging ToExternalMessaging(this DelegationEmailMessage source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new ExternalMessaging()
        {
            MessageType = source.Type,
            NotifyId = Guid.Empty,
            MessageRecipient = source.Recipient,
            TemplateId = source.TemplateId,
            RequestPayload = source.Payload.ToString(),
            CreatedAt = DateTime.UtcNow,
        };
    }
}
