// <copyright file="Application.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class Application : BaseUpdateEntity
{
    public required string Name { get; set; } = string.Empty;

    public required Guid ClientId { get; set; }

    public required string TenantName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int StatusTypeId { get; set; }

    public StatusType StatusType { get; set; }

    public ICollection<Delegation> Delegations { get; set; } = new List<Delegation>();

    public ICollection<ApplicationRole> ApplicationRoles { get; set; } = new List<ApplicationRole>();
    
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
