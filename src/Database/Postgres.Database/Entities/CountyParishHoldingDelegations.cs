// <copyright file="CountyParishHoldingDelegations.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class CountyParishHoldingDelegations : BaseAuditEntity
{
    public CountyParishHoldings CountyParishHolding { get; set; } = null!;

    public UserAccounts DelegatingUser { get; set; } = null!;

    public Guid DelegatingUserId { get; set; }

    public UserAccounts? DelegatedUser { get; set; }

    public Guid? DelegatedUserId { get; set; }

    public required string DelegatedUserEmail { get; set; }

    public Roles DelegatedUserRole { get; set; } = null!;

    public Guid DelegatedUserRoleId { get; set; }

    public string InvitationToken { get; set; } = null!;

    public DateTime InvitationExpiresAt { get; set; }

    public DateTime InvitationAcceptedAt { get; set; }

    public DateTime InvitationRejectedAt { get; set; }

    public DateTime RevokedAt { get; set; }

    public UserAccounts? RevokedByUser { get; set; }

    public Guid? RevokedById { get; set; }

    public DateTime ExpiresAt { get; set; }

    public UserAccounts CreatedByUser { get; set; } = null!;

    public UserAccounts? DeletedByUser { get; set; }
}
