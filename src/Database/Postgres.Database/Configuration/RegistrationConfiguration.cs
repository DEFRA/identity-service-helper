// <copyright file="RegistrationConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

internal class RegistrationConfiguration : BaseUpdateEntityConfiguration<Registration>
{
    public override void Configure(EntityTypeBuilder<Registration> builder)
    {
        builder.Property(x => x.CountryParishHoldingId)
            .HasColumnName(nameof(Registration.CountryParishHoldingId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.CountyParishHolding)
            .WithMany(x => x.Registrations)
            .HasForeignKey(x => x.CountryParishHoldingId)
            .IsRequired();

        builder.HasOne(x => x.Application)
            .WithMany(x => x.Registrations)
            .HasForeignKey(x => x.ApplicationId);

        builder.Property(x => x.StatusTypeId)
            .HasColumnName(nameof(Registration.StatusTypeId).ToSnakeCase())
            .HasColumnType(ColumnTypes.SmallInt)
            .HasDefaultValue(1)
            .IsRequired();

        builder.Property(x => x.EnrolledAt)
            .HasColumnName(nameof(Registration.EnrolledAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ExpiresAt)
            .HasColumnName(nameof(Registration.ExpiresAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd();

        builder.HasOne(x => x.Status)
            .WithMany(x => x.Registrations)
            .HasForeignKey(x => x.StatusTypeId);

        builder.Property(x => x.ApplicationId)
            .HasColumnName(nameof(Registration.ApplicationId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.Property(x => x.CreatedBy)
            .HasColumnName(nameof(Registration.CreatedBy).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.Property(x => x.UpdatedBy)
            .HasColumnName(nameof(Registration.UpdatedBy).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired(false);

        base.Configure(builder);
    }
}
