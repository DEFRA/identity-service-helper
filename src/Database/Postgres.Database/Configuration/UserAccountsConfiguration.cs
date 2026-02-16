// <copyright file="UserAccountsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Defra.Identity.Postgres.Database.Configuration.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class UserAccountsConfiguration : BaseAuditEntityConfiguration<UserAccounts>
{
    public override void Configure(EntityTypeBuilder<UserAccounts> builder)
    {
        builder.Property(x => x.EmailAddress)
            .HasColumnName(nameof(UserAccounts.EmailAddress).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.DisplayName)
            .HasColumnName(nameof(UserAccounts.DisplayName).ToSnakeCase())
            .HasColumnType(ColumnTypes.CiText);

        builder.Property(x => x.FirstName)
            .HasColumnName(nameof(UserAccounts.FirstName).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(256);

        builder.Property(x => x.LastName)
            .HasColumnName(nameof(UserAccounts.LastName).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(256);

        builder.Property(x => x.SamId)
            .HasColumnName(nameof(UserAccounts.SamId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired(false);

        builder.Property(x => x.KrdsId)
            .HasColumnName(nameof(UserAccounts.KrdsId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired(false);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(x => x.CreatedUsers)
            .HasForeignKey(x => x.CreatedById);

        builder.HasOne(x => x.DeletedBy)
            .WithMany(x => x.DeletedUsers)
            .HasForeignKey(x => x.DeletedById);

        builder.HasIndex(x => x.EmailAddress)
            .HasDatabaseName($"IX_{nameof(UserAccounts)}_{nameof(UserAccounts.EmailAddress)}")
            .IsUnique();

        base.Configure(builder);
    }
}
