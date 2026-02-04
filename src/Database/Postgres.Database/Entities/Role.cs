// <copyright file="Role.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class Role : BaseTypeEntity
{
    public ICollection<ApplicationRole> ApplicationRoles { get; set; } = new List<ApplicationRole>();

    public ICollection<Delegation> InvitedByRoles { get; set; } = new List<Delegation>();

    public ICollection<Delegation> DelegatedRoles { get; set; } = new List<Delegation>();
}
