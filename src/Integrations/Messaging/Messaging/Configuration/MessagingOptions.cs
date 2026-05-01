// <copyright file="MessagingOptions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Configuration;

public class MessagingOptions
{
    public required string ApiKey { get; set; }

    public string ReplyToId { get; set; } = null!;
}
