// <copyright file="RolesConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Postgres.Database.Configuration.Base;

[ExcludeFromCodeCoverage]
internal class RolesConfiguration : BaseTypeEntityConfiguration<Roles>
{
    protected override int NameMaxLength => 128;

    protected override int DescriptionMaxLength => 2048;
}
