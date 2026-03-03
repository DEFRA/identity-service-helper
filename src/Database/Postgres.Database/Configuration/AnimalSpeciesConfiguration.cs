// <copyright file="AnimalSpeciesConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

public class AnimalSpeciesConfiguration : IEntityTypeConfiguration<AnimalSpecies>
{
    public void Configure(EntityTypeBuilder<AnimalSpecies> builder)
    {
        builder.ToTable(nameof(AnimalSpecies).ToSnakeCase());
        builder.HasKey(x => new { x.Id });

        builder.Property(x => x.Id)
            .HasColumnName(nameof(AnimalSpecies.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.Char)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName(nameof(AnimalSpecies.Name).ToSnakeCase());

        builder.Property(x => x.IsActive)
            .HasColumnName(nameof(AnimalSpecies.IsActive).ToSnakeCase());
    }
}
