// <copyright file="UserAccounts.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class UserAccounts : BaseAuditEntity
{
    public string EmailAddress { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public Guid? KrdsId { get; set; }

    public string? SamId { get; set; }

    public UserAccounts? DeletedBy { get; set; }

    public UserAccounts? CreatedBy { get; set; }

    public ICollection<UserAccounts> DeletedUsers { get; set; } = new List<UserAccounts>();

    public ICollection<UserAccounts> CreatedUsers { get; set; } = new List<UserAccounts>();

    public ICollection<Applications> ApplicationsCreatedByUsers { get; set; } = new List<Applications>();

    public ICollection<Applications> ApplicationsDeletedByUsers { get; set; } = new List<Applications>();

    public ICollection<CountyParishHoldings> CountyParishHoldingsCreatedByUsers { get; set; } = new List<CountyParishHoldings>();

    public ICollection<CountyParishHoldings> CountyParishHoldingsDeletedByUsers { get; set; } = new List<CountyParishHoldings>();

    public ICollection<UserAccountCountyParishHoldingAssignments> ApplicationUserAccountHoldingAssignments { get; set; } = new List<UserAccountCountyParishHoldingAssignments>();

    public ICollection<UserAccountCountyParishHoldingAssignments> ApplicationUserAccountHoldingAssignmentsCreatedByUsers { get; set; } = new List<UserAccountCountyParishHoldingAssignments>();

    public ICollection<UserAccountCountyParishHoldingAssignments> ApplicationUserAccountHoldingAssignmentsDeletedByUsers { get; set; } = new List<UserAccountCountyParishHoldingAssignments>();

    public ICollection<CountyParishHoldingDelegations> CountyParishHoldingDelegationsDelegatingUsers { get; set; } = new List<CountyParishHoldingDelegations>();

    public ICollection<CountyParishHoldingDelegations> CountyParishHoldingDelegationsDelegatedUsers { get; set; } = new List<CountyParishHoldingDelegations>();

    public ICollection<CountyParishHoldingDelegations> CountyParishHoldingDelegationsCreatedByUsers { get; set; } = new List<CountyParishHoldingDelegations>();

    public ICollection<CountyParishHoldingDelegations> CountyParishHoldingDelegationsDeletedByUsers { get; set; } = new List<CountyParishHoldingDelegations>();

    public ICollection<CountyParishHoldingDelegations>? CountyParishHoldingDelegationsRevokedByUsers { get; set; } = new List<CountyParishHoldingDelegations>();

    public ICollection<ExternalMessaging> ExternalMessagingCreatedByUsers { get; set; } = new List<ExternalMessaging>();
}
