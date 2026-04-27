// <copyright file="ExternalMessaging.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Models;

public class ExternalMessaging
{
    public int Id { get; set; }

    public required MessageTypes MessageType { get; set; }

    public required string MessageRecipient { get; set; }

    public required Guid TemplateId { get; set; }

    public required Guid NotifyId { get; set; }

    public string? RequestPayload { get; set; }

    public DateTime? SentAt { get; set; }

    public int? ResponseCode { get; set; }

    public string? ResponseMessage { get; set; }

    public string? ExceptionMessage { get; set; }

    public required DateTime CreatedAt { get; set; }

    public Guid? CreatedById { get; set; }

    public UserAccounts? CreatedByUser { get; set; }

    public ICollection<CountyParishHoldingDelegationsNotifications> CountyParishHoldingDelegationsNotifications { get; set; } = new List<CountyParishHoldingDelegationsNotifications>();
}
