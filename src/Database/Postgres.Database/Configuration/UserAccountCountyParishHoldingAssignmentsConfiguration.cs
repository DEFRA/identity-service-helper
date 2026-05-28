// <copyright file="UserAccountCountyParishHoldingAssignmentsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Postgres.Database.Configuration.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[ExcludeFromCodeCoverage]
internal class UserAccountCountyParishHoldingAssignmentsConfiguration
    : BaseAuditEntityConfiguration<UserAccountCountyParishHoldingAssignments>
{
    public override void Configure(EntityTypeBuilder<UserAccountCountyParishHoldingAssignments> builder)
    {
        builder.Property(x => x.CountyParishHoldingId)
            .HasColumnName(nameof(UserAccountCountyParishHoldingAssignments.CountyParishHoldingId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.CountyParishHolding)
            .WithMany(x => x.ApplicationUserAccountHoldingAssignments)
            .HasForeignKey(x => x.CountyParishHoldingId);

        builder.Property(x => x.RoleId)
            .HasColumnName(nameof(UserAccountCountyParishHoldingAssignments.RoleId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.Role)
            .WithMany(x => x.ApplicationUserAccountHoldingAssignments)
            .HasForeignKey(x => x.RoleId);

        builder.Property(x => x.UserAccountId)
            .HasColumnName(nameof(UserAccountCountyParishHoldingAssignments.UserAccountId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.UserAccount)
            .WithMany(x => x.ApplicationUserAccountHoldingAssignments)
            .HasForeignKey(x => x.UserAccountId);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.ApplicationUserAccountHoldingAssignmentsCreatedByUsers)
            .HasForeignKey(x => x.CreatedById);

        builder.HasOne(x => x.DeletedByUser)
            .WithMany(x => x.ApplicationUserAccountHoldingAssignmentsDeletedByUsers)
            .HasForeignKey(x => x.DeletedById);

        base.Configure(builder);
    }
}
