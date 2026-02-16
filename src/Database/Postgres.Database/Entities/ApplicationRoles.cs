// <copyright file="ApplicationRoles.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

public class ApplicationRoles
{
    public Guid ApplicationId { get; set; }

    public required Applications Applications { get; set; }

    public Guid RoleId { get; set; }

    public required Roles Roles { get; set; }
}
