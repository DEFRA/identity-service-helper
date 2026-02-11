// <copyright file="UserAccount.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class UserAccount : BaseUpdateEntity
{
    public string EmailAddress { get; set; } = string.Empty;

    public int StatusTypeId { get; set; }

    public StatusType Status { get; set; } = null!;

    public string DisplayName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public Guid CreatedBy { get; set; }

    public Guid? UpdatedBy { get; set; }

    // Self-referencing navigations (parent users)
    public UserAccount? CreatedByUser { get; set; }

    public UserAccount? UpdatedByUser { get; set; }

    public ICollection<UserAccount> CreatedUsers { get; set; } = new List<UserAccount>();

    public ICollection<UserAccount> UpdatedUsers { get; set; } = new List<UserAccount>();

    public ICollection<Federation> Federations { get; set; } = new List<Federation>();

    public ICollection<Delegation> Delegations { get; set; } = new List<Delegation>();

    public ICollection<Delegation> InvitedByUsers { get; set; } = new List<Delegation>();
}
