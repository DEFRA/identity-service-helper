// <copyright file="UserAccountConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgre.Database.Configuration;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class UserAccountConfiguration : BaseUpdateEntityConfiguration<Applications>
{
    public override void Configure(EntityTypeBuilder<Applications> builder)
    {
        builder.HasIndex(x => x.Upn);

        builder.Property(x => x.Upn)
            .HasColumnName(nameof(Applications.Upn).ToSnakeCase())
            .HasColumnType(ColumnTypes.CiText);

        builder.Property(x => x.DisplayName)
            .HasColumnName(nameof(Applications.DisplayName).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(256);

        builder.Property(x => x.AccountEnabled)
            .HasColumnName(nameof(Applications.AccountEnabled).ToSnakeCase())
            .HasColumnType(ColumnTypes.Boolean)
            .HasDefaultValue(true);

        base.Configure(builder);
    }
}
