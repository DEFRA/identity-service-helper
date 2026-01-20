// <copyright file="Application.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class Application : BaseUpdateEntity
{
    public required string Name { get; set; } = string.Empty;

    public required Guid ClientId { get; set; }

    public required string TenantName { get; set; }

    public string Description { get; set; }

    public int StatusTypeId { get; set; }

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();

    public ICollection<ApplicationRole> ApplicationRoles { get; set; } = new List<ApplicationRole>();
}
