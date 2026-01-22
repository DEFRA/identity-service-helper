// <copyright file="ApplicationConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using Defra.Identity.Postgres.Database.Entities.Base;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace Defra.Identity.Postgres.Database.Configuration;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class ApplicationConfiguration
    : BaseUpdateEntityConfiguration<Application>
{
    public override void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.Property(x => x.Name)
            .HasColumnName(nameof(Application.Name).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .HasComment("Human readable name for the application e.g Keeper Portal")
            .IsRequired();

        builder.Property(x => x.ClientId)
            .HasColumnName(nameof(Application.ClientId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .HasComment("Azure AD B2C application Client ID")
            .IsRequired();

        builder.Property(x => x.TenantName)
            .HasColumnName(nameof(Application.TenantName).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .HasComment("Azure AD B2C tenant name e.g defra.onmicrosoft.com")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName(nameof(Application.Description).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text);

        builder.Property(x => x.StatusTypeId)
            .HasColumnName(nameof(Application.StatusTypeId).ToSnakeCase())
            .HasColumnType(ColumnTypes.SmallInt)
            .HasDefaultValue(1)
            .IsRequired();

        builder.HasOne(x => x.StatusType)
            .WithMany(a => a.Applications)
            .HasForeignKey(x => x.StatusTypeId);

        base.Configure(builder);
    }
}
