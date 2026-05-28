// <copyright file="CountyParishHoldingAnimalSpeciesConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[ExcludeFromCodeCoverage]
internal class CountyParishHoldingAnimalSpeciesConfiguration : IEntityTypeConfiguration<CountyParishHoldingAnimalSpecies>
{
    public void Configure(EntityTypeBuilder<CountyParishHoldingAnimalSpecies> builder)
    {
        builder.ToTable(nameof(CountyParishHoldingAnimalSpecies).ToSnakeCase());
        builder.HasKey(x => new { x.CountyParishHoldingId, x.AnimalSpeciesId });

        builder.Property(x => x.CountyParishHoldingId)
            .HasColumnName(nameof(CountyParishHoldingDelegations.CountyParishHoldingId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.CountyParishHolding)
            .WithMany(x => x.CountyParishHoldingAnimalSpecies)
            .HasForeignKey(x => x.CountyParishHoldingId);

        builder.Property(x => x.AnimalSpeciesId)
            .HasColumnName(nameof(CountyParishHoldingAnimalSpecies.AnimalSpeciesId).ToSnakeCase())
            .IsRequired();

        builder.HasOne(x => x.AnimalSpecies)
            .WithMany(x => x.CountyParishHoldingAnimalSpecies)
            .HasForeignKey(x => x.AnimalSpeciesId);
    }
}
