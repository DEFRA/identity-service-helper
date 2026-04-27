// <copyright file="MessageStatus.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Messaging;

public class MessageStatus
{
    public string NotifyId { get; set; }

    public string Recipient { get; set; }

    public string MessageType { get; set; }

    public string TemplateId { get; set; }

    public string Status { get; set; }
}
