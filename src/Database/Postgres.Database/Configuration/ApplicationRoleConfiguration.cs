// <copyright file="ApplicationRoleConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable(nameof(ApplicationRole).ToSnakeCase());
        builder.HasKey(x => new { x.ApplicationId, x.RoleId });

        builder.Property(x => x.ApplicationId)
            .HasColumnName(nameof(ApplicationRole.ApplicationId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.Property(x => x.RoleId)
            .HasColumnName(nameof(ApplicationRole.RoleId).ToSnakeCase())
            .HasColumnType(ColumnTypes.SmallInt);

        builder.HasOne(x => x.Application)
            .WithMany(e => e.ApplicationRoles)
            .HasForeignKey(x => x.ApplicationId)
            .HasConstraintName("application_role_mapping_application_id_fk");

        builder.HasOne(x => x.Role)
            .WithMany(e => e.ApplicationRoles)
            .HasForeignKey(x => x.RoleId)
            .HasConstraintName("application_role_mapping_role_id_fk");
    }
}
