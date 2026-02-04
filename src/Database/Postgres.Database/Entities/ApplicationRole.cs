// <copyright file="ApplicationRole.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

public class ApplicationRole
{
    public Guid ApplicationId { get; set; }

    public Application Application { get; set; }

    public int RoleId { get; set; }

    public Role Role { get; set; }
}
