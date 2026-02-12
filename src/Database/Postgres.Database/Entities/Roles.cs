// <copyright file="Roles.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class Roles : BaseTypeEntity
{
    public ICollection<ApplicationRoles> ApplicationRoles { get; set; } = new List<ApplicationRoles>();

    public ICollection<Delegations> InvitedByRoles { get; set; } = new List<Delegations>();

    public ICollection<Delegations> DelegatedRoles { get; set; } = new List<Delegations>();

    public ICollection<DelegationInvitations> DelegationInvitations { get; set; } = new List<DelegationInvitations>();

    public ICollection<ApplicationUserAccountHoldingAssignments> ApplicationUserAccountHoldingAssignments { get; set; } = new List<ApplicationUserAccountHoldingAssignments>();
}
