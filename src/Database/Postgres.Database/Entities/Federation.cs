// <copyright file="Federation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class Federation : BaseUpdateEntity
{
    public required Guid UserAccountId { get; set; }

    public required UserAccount UserAccount { get; set; }

    public required string TenantName { get; set; }

    public required Guid ObjectId { get; set; }

    public required string TrustLevel { get; set; }

    public required string SyncStatus { get; set; }

    public required DateTime LastSyncedAt { get; set; }
}
