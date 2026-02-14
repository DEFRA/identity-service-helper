// <copyright file="DelegationInvitations.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using System.Text.Json;
using Defra.Identity.Postgres.Database.Entities.Base;

public class DelegationInvitations : BaseAuditEntity
{
    public Guid DelegationId { get; set; }

    public Delegations Delegation { get; set; } = null!;

    public Guid InvitedUserId { get; set; }

    public string InvitedEmail { get; set; } = null!;

    public string InvitationToken { get; set; } = null!;

    public DateTime TokenExpiresAt { get; set; }

    public Guid DelegatedRoleId { get; set; }

    public Roles DelegatedRole { get; set; } = null!;

    public JsonDocument? DelegatedPermissions { get; set; }

    public DateTime InvitedAt { get; set; }

    public DateTime AcceptedAt { get; set; }

    public DateTime RegisteredAt { get; set; }

    public DateTime ActivatedAt { get; set; }

    public DateTime RevokedAt { get; set; }

    public DateTime ExpiredAt { get; set; }

    public UserAccounts? DeletedByUser { get; set; }

    public UserAccounts CreatedByUser { get; set; } = null!;
}
