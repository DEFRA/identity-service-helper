// <copyright file="DelegationInvitationsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Defra.Identity.Postgres.Database.Configuration.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class DelegationInvitationsConfiguration : BaseAuditEntityConfiguration<DelegationInvitations>
{
    public override void Configure(EntityTypeBuilder<DelegationInvitations> builder)
    {
        // Foreign keys
        builder.Property(x => x.DelegationId)
            .HasColumnName(nameof(DelegationInvitations.DelegationId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.Delegation)
            .WithMany(x => x.DelegationInvitations)
            .HasForeignKey(x => x.DelegationId);

        builder.Property(x => x.InvitedUserId)
            .HasColumnName(nameof(DelegationInvitations.InvitedUserId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.Property(x => x.DelegatedRoleId)
            .HasColumnName(nameof(DelegationInvitations.DelegatedRoleId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.DelegatedRole)
            .WithMany(x => x.DelegationInvitations)
            .HasForeignKey(x => x.DelegatedRoleId);

        // Strings
        builder.Property(x => x.InvitedEmail)
            .HasColumnName(nameof(DelegationInvitations.InvitedEmail).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(256);

        builder.Property(x => x.InvitationToken)
            .HasColumnName(nameof(DelegationInvitations.InvitationToken).ToSnakeCase())
            .HasColumnType(ColumnTypes.Char)
            .HasMaxLength(64);

        builder.Property(x => x.DelegatedPermissions)
            .HasColumnName(nameof(DelegationInvitations.DelegatedPermissions).ToSnakeCase())
            .HasColumnType(ColumnTypes.JsonB);

        // Date/times
        builder.Property(x => x.TokenExpiresAt)
            .HasColumnName(nameof(DelegationInvitations.TokenExpiresAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.InvitedAt)
            .HasColumnName(nameof(DelegationInvitations.InvitedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.AcceptedAt)
            .HasColumnName(nameof(DelegationInvitations.AcceptedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.RegisteredAt)
            .HasColumnName(nameof(DelegationInvitations.RegisteredAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.ActivatedAt)
            .HasColumnName(nameof(DelegationInvitations.ActivatedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.RevokedAt)
            .HasColumnName(nameof(DelegationInvitations.RevokedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.ExpiredAt)
            .HasColumnName(nameof(DelegationInvitations.ExpiredAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.DelegationInvitationsCreatedByUsers)
            .HasForeignKey(x => x.CreatedById);

        builder.HasOne(x => x.DeletedByUser)
            .WithMany(x => x.DelegationInvitationsDeletedByUsers)
            .HasForeignKey(x => x.DeletedById);

        base.Configure(builder);
    }
}
