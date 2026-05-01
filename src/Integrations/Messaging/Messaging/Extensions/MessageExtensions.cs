// <copyright file="MessageExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Models.Request;

using Defra.Identity.Postgres.Database.Entities;
using Newtonsoft.Json;

public static class MessageExtensions
{
    public static Message ToMessage(this ExternalMessaging source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = new Message(source.MessageType, source.TemplateId) { Recipient = source.MessageRecipient, };

        if (source.RequestPayload != null)
        {
            result.Payload = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(source.RequestPayload)!;
        }

        return result;
    }
}
