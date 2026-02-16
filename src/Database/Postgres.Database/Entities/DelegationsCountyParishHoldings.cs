// <copyright file="DelegationsCountyParishHoldings.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class DelegationsCountyParishHoldings : BaseAuditEntity
{
    public Guid DelegationId { get; set; }

    public Delegations Delegation { get; set; } = null!;

    public Guid CountyParishHoldingId { get; set; }

    public CountyParishHoldings CountyParishHolding { get; set; } = null!;

    public UserAccounts? DeletedByUser { get; set; }

    public UserAccounts CreatedByUser { get; set; } = null!;
}
