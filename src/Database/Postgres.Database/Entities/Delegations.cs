// <copyright file="Delegations.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class Delegations : BaseAuditEntity
{
    public Guid ApplicationId { get; set; }

    public Applications Applications { get; set; } = null!;

    public Guid UserId { get; set; }

    public UserAccounts UserAccount { get; set; } = null!;

    public ICollection<DelegationsCountyParishHoldings> DelegationsCountyParishHoldings { get; set; } = new List<DelegationsCountyParishHoldings>();

    public ICollection<DelegationInvitations> DelegationInvitations { get; set; } = new List<DelegationInvitations>();
}
