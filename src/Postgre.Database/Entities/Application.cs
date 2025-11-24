// <copyright file="Application.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgre.Database.Entities;

using Defra.Identity.Postgre.Database.Entities.Base;

public class Application : BaseUpdateEntity
{
    public required string Name { get; set; } = string.Empty;

    public required Guid ClientId { get; set; }

    public required string TenantName { get; set; }

    public string Description { get; set; }

    public string Status { get; set; }

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
