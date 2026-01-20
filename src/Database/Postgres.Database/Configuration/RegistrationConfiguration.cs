// <copyright file="RegistrationConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

internal class RegistrationConfiguration : BaseUpdateEntityConfiguration<Registration>
{
    public override void Configure(EntityTypeBuilder<Registration> builder)
    {
        builder.Property(x => x.CreatedBy)
            .HasColumnName(nameof(Registration.CreatedBy).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.Property(x => x.UpdatedBy)
            .HasColumnName(nameof(Registration.UpdatedBy).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        base.Configure(builder);
    }
}
