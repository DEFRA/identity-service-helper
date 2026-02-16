// <copyright file="ApplicationsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Defra.Identity.Postgres.Database.Configuration.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class ApplicationsConfiguration
    : BaseAuditEntityConfiguration<Applications>
{
    public override void Configure(EntityTypeBuilder<Applications> builder)
    {
        builder.Property(x => x.Name)
            .HasColumnName(nameof(Applications.Name).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .HasComment("Human readable name for the application e.g Keeper Portal")
            .IsRequired();

        builder.Property(x => x.ClientId)
            .HasColumnName(nameof(Applications.ClientId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .HasComment("Azure AD B2C application Client ID")
            .IsRequired();

        builder.Property(x => x.TenantName)
            .HasColumnName(nameof(Applications.TenantName).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .HasComment("Azure AD B2C tenant name e.g defra.onmicrosoft.com")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName(nameof(Applications.Description).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.ApplicationsCreatedByUsers)
            .HasForeignKey(x => x.CreatedById);

        builder.HasOne(x => x.DeletedByUser)
            .WithMany(x => x.ApplicationsDeletedByUsers)
            .HasForeignKey(x => x.DeletedById);

        base.Configure(builder);
    }
}
