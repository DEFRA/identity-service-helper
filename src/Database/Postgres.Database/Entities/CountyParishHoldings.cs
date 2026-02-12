// <copyright file="CountyParishHoldings.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class CountyParishHoldings : BaseAuditEntity
{
    public string Identifier { get; set; } = string.Empty;

    public DateTime? ExpiredAt { get; set; }

    public UserAccounts? DeletedByUser { get; set; }

    public UserAccounts CreatedByUser { get; set; } = null!;

    public ICollection<Delegations> Delegations { get; set; } = new List<Delegations>();

    public ICollection<DelegationsCountyParishHoldings> DelegationsCountyParishHoldings { get; set; } = new List<DelegationsCountyParishHoldings>();

    public ICollection<ApplicationUserAccountHoldingAssignments> ApplicationUserAccountHoldingAssignments { get; set; } = new List<ApplicationUserAccountHoldingAssignments>();
}
