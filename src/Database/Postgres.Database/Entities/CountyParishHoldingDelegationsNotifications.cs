// <copyright file="CountyParishHoldingDelegationsNotifications.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

public class CountyParishHoldingDelegationsNotifications
{
    public Guid DelegationId { get; set; }

    public CountyParishHoldingDelegations Delegation { get; set; } = null!;

    public int MessageId { get; set; }

    public ExternalMessaging Message { get; set; } = null!;
}
