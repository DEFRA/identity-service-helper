// <copyright file="Message.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Models.Request;

using Defra.Identity.Postgres.Database;

public class Message(
    MessageTypes type,
    Guid templateId)
{
    public MessageTypes Type { get; } = type;

    public Guid TemplateId { get; } = templateId;

    public string Recipient { get; set; } = string.Empty;

    public Dictionary<string, dynamic> Payload { get; set; } = new();
}
