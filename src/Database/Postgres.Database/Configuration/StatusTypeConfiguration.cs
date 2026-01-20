// <copyright file="StatusTypeConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

internal class StatusTypeConfiguration : BaseTypeEntityConfiguration<StatusType>
{
    protected override int NameMaxLength => 10;

    protected override int DescriptionMaxLength => 250;
}
