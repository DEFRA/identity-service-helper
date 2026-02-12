// <copyright file="Applications.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class Applications : BaseAuditEntity
{
    public required string Name { get; set; } = string.Empty;

    public required Guid ClientId { get; set; }

    public required string TenantName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public UserAccounts? DeletedByUser { get; set; }

    public UserAccounts CreatedByUser { get; set; } = null!;

    public ICollection<Delegations> Delegations { get; set; } = new List<Delegations>();

    public ICollection<ApplicationRoles> ApplicationRoles { get; set; } = new List<ApplicationRoles>();

    public ICollection<ApplicationUserAccountHoldingAssignments> ApplicationUserAccountHoldingAssignments { get; set; } = new List<ApplicationUserAccountHoldingAssignments>();
}
