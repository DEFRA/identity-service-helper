// <copyright file="RoleConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

internal class RoleConfiguration : BaseTypeEntityConfiguration<Role>
{
    protected override int NameMaxLength => 128;

    protected override int DescriptionMaxLength => 2048;
}
