// <copyright file="ApplicationUserAccountHoldingAssignments.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class ApplicationUserAccountHoldingAssignments : BaseAuditEntity
{
    public Guid CountyParishHoldingId { get; set; }

    public CountyParishHoldings Type { get; set; } = null!;

    public Guid ApplicationId { get; set; }

    public Applications Application { get; set; } = null!;

    public Guid RoleId { get; set; }

    public Roles Role { get; set; } = null!;

    public Guid UserAccountId { get; set; }

    public UserAccounts UserAccount { get; set; } = null!;

    public UserAccounts? DeletedByUser { get; set; }

    public UserAccounts CreatedByUser { get; set; } = null!;
}
