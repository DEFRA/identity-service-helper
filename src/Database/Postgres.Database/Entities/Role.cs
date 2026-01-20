// <copyright file="Role.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class Role : BaseTypeEntity
{
    public ICollection<ApplicationRole> ApplicationRoles { get; set; } = new List<ApplicationRole>();
}
