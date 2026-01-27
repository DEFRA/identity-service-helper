// <copyright file="EnrolmentConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class DelegationConfiguration
    : BaseUpdateEntityConfiguration<Delegation>
{
    public override void Configure(EntityTypeBuilder<Delegation> builder)
    {
        builder.HasIndex(x => x.CountyParishHoldingId);

        builder.Property(x => x.DelegatedPermissions)
            .HasColumnName(nameof(Delegation.DelegatedPermissions).ToSnakeCase())
            .HasColumnType(ColumnTypes.JsonB);

        builder.Property(x => x.StatusTypeId)
            .HasColumnName(nameof(Delegation.StatusTypeId).ToSnakeCase())
            .HasColumnType(ColumnTypes.SmallInt)
            .HasDefaultValue(1);

        builder.Property(x => x.CountyParishHoldingId)
            .HasColumnName(nameof(Delegation.CountyParishHoldingId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.Property(x => x.ApplicationId)
            .HasColumnName(nameof(Delegation.ApplicationId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.Property(x => x.InvitedUserId)
            .HasColumnName(nameof(Delegation.InvitedUserId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.Property(x => x.InvitedByUserId)
            .HasColumnName(nameof(Delegation.InvitedByUserId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.Property(x => x.InvitedByRoleId)
            .HasColumnName(nameof(Delegation.InvitedByRoleId).ToSnakeCase())
            .HasColumnType(ColumnTypes.SmallInt)
            .IsRequired();

        builder.Property(x => x.InvitedEmail)
            .HasColumnName(nameof(Delegation.InvitedEmail).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.InvitationToken)
            .HasColumnName(nameof(Delegation.InvitationToken).ToSnakeCase())
            .HasColumnType(ColumnTypes.Char)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.TokenExpiresAt)
            .HasColumnName(nameof(Delegation.TokenExpiresAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Char)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.DelegatedRoleId)
            .HasColumnName(nameof(Delegation.DelegatedRoleId).ToSnakeCase())
            .HasColumnType(ColumnTypes.SmallInt)
            .IsRequired();

        builder.Property(x => x.InvitedAt)
            .HasColumnName(nameof(Delegation.InvitedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .IsRequired();

        builder.Property(x => x.AcceptedAt)
            .HasColumnName(nameof(Delegation.AcceptedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.RegisteredAt)
            .HasColumnName(nameof(Delegation.RegisteredAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.AcceptedAt)
            .HasColumnName(nameof(Delegation.ActivatedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.RevokedAt)
            .HasColumnName(nameof(Delegation.RevokedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.ExpiredAt)
            .HasColumnName(nameof(Delegation.ExpiredAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.CreatedAt)
            .HasColumnName(nameof(Delegation.CreatedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.CreatedBy)
            .HasColumnName(nameof(Delegation.CreatedBy).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.Property(x => x.DelegatedPermissions)
            .HasColumnName(nameof(Delegation.DelegatedPermissions).ToSnakeCase())
            .HasColumnType(ColumnTypes.JsonB);

        builder.HasOne(x => x.InvitedByRole)
            .WithMany(x => x.InvitedByRoles)
            .HasForeignKey(x => x.InvitedByRoleId);

        builder.HasOne(x => x.DelegatedRole)
            .WithMany(x => x.DelegatedRoles)
            .HasForeignKey(x => x.DelegatedRoleId);

        builder.HasOne(x => x.Application)
            .WithMany(x => x.Delegations)
            .HasForeignKey(x => x.ApplicationId);

        builder.HasOne(x => x.InvitedByUser)
            .WithMany(x => x.InvitedByUsers)
            .HasForeignKey(x => x.InvitedByUserId);

        builder.HasOne(x => x.Status)
            .WithMany(x => x.Delegations)
            .HasForeignKey(x => x.StatusTypeId);

        builder.HasOne(x => x.CountyParishHolding)
            .WithMany(x => x.Delegations)
            .HasForeignKey(x => x.CountyParishHoldingId);

        base.Configure(builder);
    }
}
