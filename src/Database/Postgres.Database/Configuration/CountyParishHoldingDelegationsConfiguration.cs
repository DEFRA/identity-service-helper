// <copyright file="CountyParishHoldingDelegationsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Defra.Identity.Postgres.Database.Configuration.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class CountyParishHoldingDelegationsConfiguration : BaseAuditEntityConfiguration<CountyParishHoldingDelegations>
{
    public override void Configure(EntityTypeBuilder<CountyParishHoldingDelegations> builder)
    {
        builder.Property(x => x.CountyParishHoldingId)
            .HasColumnName(nameof(CountyParishHoldingDelegations.CountyParishHoldingId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.CountyParishHolding)
            .WithMany(x => x.DelegationsCountyParishHoldings)
            .HasForeignKey(x => x.CountyParishHoldingId);

        builder.Property(x => x.DelegatingUserId)
            .HasColumnName(nameof(CountyParishHoldingDelegations.DelegatingUserId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.DelegatingUser)
            .WithMany(x => x.CountyParishHoldingDelegationsDelegatingUsers)
            .HasForeignKey(x => x.DelegatingUserId);

        builder.Property(x => x.DelegatedUserId)
            .HasColumnName(nameof(CountyParishHoldingDelegations.DelegatedUserId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired(false);

        builder.HasOne(x => x.DelegatedUser)
            .WithMany(x => x.CountyParishHoldingDelegationsDelegatedUsers)
            .HasForeignKey(x => x.DelegatedUserId);

        builder.HasOne(x => x.DelegatedUserRole)
            .WithMany(x => x.CountyParishHoldingDelegationsUserRoles)
            .HasForeignKey(x => x.DelegatedUserRoleId);

        builder.Property(x => x.DelegatedUserEmail)
            .HasColumnName(nameof(CountyParishHoldingDelegations.DelegatedUserEmail).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.DelegatedUserRoleId)
            .HasColumnName(nameof(CountyParishHoldingDelegations.DelegatedUserRoleId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.DelegatedUserRole)
            .WithMany(x => x.CountyParishHoldingDelegationsUserRoles)
            .HasForeignKey(x => x.DelegatedUserRoleId);

        builder.Property(x => x.InvitationToken)
            .HasColumnName(nameof(CountyParishHoldingDelegations.InvitationToken).ToSnakeCase())
            .HasColumnType(ColumnTypes.Char)
            .HasMaxLength(64);

        builder.Property(x => x.InvitationExpiresAt)
            .HasColumnName(nameof(CountyParishHoldingDelegations.InvitationExpiresAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.InvitationAcceptedAt)
            .HasColumnName(nameof(CountyParishHoldingDelegations.InvitationAcceptedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.InvitationRejectedAt)
            .HasColumnName(nameof(CountyParishHoldingDelegations.InvitationRejectedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.RevokedAt)
            .HasColumnName(nameof(CountyParishHoldingDelegations.RevokedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.RevokedById)
            .HasColumnName(nameof(CountyParishHoldingDelegations.RevokedById).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.HasOne(x => x.RevokedByUser)
            .WithMany(x => x.CountyParishHoldingDelegationsRevokedByUsers)
            .HasForeignKey(x => x.RevokedById);

        builder.Property(x => x.ExpiresAt)
            .HasColumnName(nameof(CountyParishHoldingDelegations.ExpiresAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CountyParishHoldingDelegationsCreatedByUsers)
            .HasForeignKey(x => x.CreatedById);

        builder.HasOne(x => x.DeletedByUser)
            .WithMany(x => x.CountyParishHoldingDelegationsDeletedByUsers)
            .HasForeignKey(x => x.DeletedById);

        base.Configure(builder);
    }
}
