// <copyright file="ApplicationRolesConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

public class ApplicationRolesConfiguration : IEntityTypeConfiguration<ApplicationRoles>
{
    public void Configure(EntityTypeBuilder<ApplicationRoles> builder)
    {
        builder.ToTable(nameof(ApplicationRoles).ToSnakeCase());
        builder.HasKey(x => new { x.ApplicationId, x.RoleId });

        builder.Property(x => x.ApplicationId)
            .HasColumnName(nameof(ApplicationRoles.ApplicationId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.Property(x => x.RoleId)
            .HasColumnName(nameof(ApplicationRoles.RoleId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.HasOne(x => x.Applications)
            .WithMany(e => e.ApplicationRoles)
            .HasForeignKey(x => x.ApplicationId)
            .HasConstraintName("application_role_mapping_application_id_fk");

        builder.HasOne(x => x.Roles)
            .WithMany(e => e.ApplicationRoles)
            .HasForeignKey(x => x.RoleId)
            .HasConstraintName("application_role_mapping_role_id_fk");
    }
}
