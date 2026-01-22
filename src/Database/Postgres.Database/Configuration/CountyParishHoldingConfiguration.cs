// <copyright file="CountyParishHoldingConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

public class CountyParishHoldingConfiguration : BaseUpdateEntityConfiguration<CountyParishHolding>
{
    public override void Configure(EntityTypeBuilder<CountyParishHolding> builder)
    {
        builder.ToTable(nameof(CountyParishHolding).ToSnakeCase());

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName(nameof(CountyParishHolding.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(x => x.StatusTypeId)
            .HasColumnName(nameof(CountyParishHolding.StatusTypeId).ToSnakeCase())
            .HasColumnType(ColumnTypes.SmallInt)
            .HasDefaultValue(1)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasColumnName(nameof(CountyParishHolding.CreatedBy).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .HasColumnName(nameof(CountyParishHolding.UpdatedBy).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.HasOne(x => x.StatusType)
            .WithMany(x => x.CountyParishHoldings)
            .HasForeignKey(x => x.StatusTypeId);

        base.Configure(builder);
    }
}
