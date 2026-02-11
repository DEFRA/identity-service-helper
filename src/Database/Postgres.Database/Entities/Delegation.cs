// <copyright file="Delegation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class Delegation : BaseUpdateEntity
{
    public int StatusTypeId { get; set; }

    public StatusType Status { get; set; } = null!;

    public Guid CountyParishHoldingId { get; set; }

    public CountyParishHolding CountyParishHolding { get; set; } = null!;

    public Guid ApplicationId { get; set; }

    public Application Application { get; set; } = null!;

    public Guid InvitedByUserId { get; set; }

    public UserAccount InvitedByUser { get; set; } = null!;

    public int InvitedByRoleId { get; set; }

    public Role InvitedByRole { get; set; } = null!;

    public Guid InvitedUserId { get; set; }

    public UserAccount InvitedUser { get; set; } = null!;

    public string InvitedEmail { get; set; } = string.Empty;

    public string InvitationToken { get; set; } = string.Empty;

    public DateTime TokenExpiresAt { get; set; }

    public int DelegatedRoleId { get; set; }

    public Role DelegatedRole { get; set; } = null!;

    public object DelegatedPermissions { get; set; } = null!;

    public DateTime InvitedAt { get; set; }

    public DateTime AcceptedAt { get; set; }

    public DateTime RegisteredAt { get; set; }

    public DateTime RevokedAt { get; set; }

    public DateTime ActivatedAt { get; set; }

    public DateTime ExpiredAt { get; set; }

    public Guid CreatedBy { get; set; }
}
