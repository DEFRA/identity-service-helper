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

    public Guid? SamId { get; set; }

    public UserAccounts? DeletedBy { get; set; }

    public UserAccounts? CreatedBy { get; set; }

    public ICollection<UserAccounts> DeletedUsers { get; set; } = new List<UserAccounts>();

    public ICollection<UserAccounts> CreatedUsers { get; set; } = new List<UserAccounts>();

    public ICollection<Applications> ApplicationsCreatedByUsers { get; set; } = new List<Applications>();

    public ICollection<Applications> ApplicationsDeletedByUsers { get; set; } = new List<Applications>();

    public ICollection<CountyParishHoldings> CountyParishHoldingsCreatedByUsers { get; set; } = new List<CountyParishHoldings>();

    public ICollection<CountyParishHoldings> CountyParishHoldingsDeletedByUsers { get; set; } = new List<CountyParishHoldings>();

    public ICollection<ApplicationUserAccountHoldingAssignments> ApplicationUserAccountHoldingAssignments { get; set; } = new List<ApplicationUserAccountHoldingAssignments>();

    public ICollection<ApplicationUserAccountHoldingAssignments> ApplicationUserAccountHoldingAssignmentsCreatedByUsers { get; set; } = new List<ApplicationUserAccountHoldingAssignments>();

    public ICollection<ApplicationUserAccountHoldingAssignments> ApplicationUserAccountHoldingAssignmentsDeletedByUsers { get; set; } = new List<ApplicationUserAccountHoldingAssignments>();

    public ICollection<CountyParishHoldingDelegations> CountyParishHoldingDelegationsDelegatingUsers { get; set; } = new List<CountyParishHoldingDelegations>();

    public ICollection<CountyParishHoldingDelegations> CountyParishHoldingDelegationsDelegatedUsers { get; set; } = new List<CountyParishHoldingDelegations>();

    public ICollection<CountyParishHoldingDelegations> CountyParishHoldingDelegationsCreatedByUsers { get; set; } = new List<CountyParishHoldingDelegations>();

    public ICollection<CountyParishHoldingDelegations> CountyParishHoldingDelegationsDeletedByUsers { get; set; } = new List<CountyParishHoldingDelegations>();

    public ICollection<CountyParishHoldingDelegations>? CountyParishHoldingDelegationsRevokedByUsers { get; set; } = new List<CountyParishHoldingDelegations>();
}
