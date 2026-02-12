// <copyright file="ApplicationUserAccountHoldingAssignmentsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Defra.Identity.Postgres.Database.Configuration.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class ApplicationUserAccountHoldingAssignmentsConfiguration
    : BaseAuditEntityConfiguration<ApplicationUserAccountHoldingAssignments>
{
    public override void Configure(EntityTypeBuilder<ApplicationUserAccountHoldingAssignments> builder)
    {
        builder.Property(x => x.CountyParishHoldingId)
            .HasColumnName(nameof(ApplicationUserAccountHoldingAssignments.CountyParishHoldingId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.Type)
            .WithMany(x => x.ApplicationUserAccountHoldingAssignments)
            .HasForeignKey(x => x.CountyParishHoldingId);

        builder.Property(x => x.ApplicationId)
            .HasColumnName(nameof(ApplicationUserAccountHoldingAssignments.ApplicationId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.Application)
            .WithMany(x => x.ApplicationUserAccountHoldingAssignments)
            .HasForeignKey(x => x.ApplicationId);

        builder.Property(x => x.RoleId)
            .HasColumnName(nameof(ApplicationUserAccountHoldingAssignments.RoleId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.Role)
            .WithMany(x => x.ApplicationUserAccountHoldingAssignments)
            .HasForeignKey(x => x.RoleId);

        builder.Property(x => x.UserAccountId)
            .HasColumnName(nameof(ApplicationUserAccountHoldingAssignments.UserAccountId).ToSnakeCase())
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
