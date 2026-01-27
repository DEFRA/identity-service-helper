// <copyright file="UserAccountConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class UserAccountConfiguration : BaseUpdateEntityConfiguration<UserAccount>
{
    public override void Configure(EntityTypeBuilder<UserAccount> builder)
    {

        builder.Property(x => x.EmailAddress)
            .HasColumnName(nameof(UserAccount.EmailAddress).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(x => x.EmailAddress).IsUnique();

        builder.Property(x => x.StatusTypeId)
            .HasColumnName(nameof(UserAccount.StatusTypeId).ToSnakeCase())
            .HasColumnType(ColumnTypes.SmallInt)
            .HasDefaultValue(1)
            .IsRequired();

        builder.HasOne(x => x.Status)
            .WithMany(e => e.UserAccounts)
            .HasForeignKey(x => x.StatusTypeId);

        builder.Property(x => x.DisplayName)
            .HasColumnName(nameof(UserAccount.DisplayName).ToSnakeCase())
            .HasColumnType(ColumnTypes.CiText);

        builder.Property(x => x.FirstName)
            .HasColumnName(nameof(UserAccount.FirstName).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(256);

        builder.Property(x => x.LastName)
            .HasColumnName(nameof(UserAccount.LastName).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(256);

        builder.Property(x => x.CreatedBy)
            .HasColumnName(nameof(UserAccount.CreatedBy).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.Property(x => x.UpdatedBy)
            .HasColumnName(nameof(UserAccount.UpdatedBy).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired(false);

        // FK: user_account.created_by -> user_account.id
        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CreatedUsers)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        // FK: user_account.updated_by -> user_account.id
        builder.HasOne(x => x.UpdatedByUser)
            .WithMany(x => x.UpdatedUsers)
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        // Optional: indexes help FK lookups
        builder.HasIndex(x => x.CreatedBy);
        builder.HasIndex(x => x.UpdatedBy);

        base.Configure(builder);
    }
}
