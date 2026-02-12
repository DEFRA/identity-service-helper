// <copyright file="CountyParishHoldingsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Defra.Identity.Postgres.Database.Configuration.Base;

public class CountyParishHoldingsConfiguration : BaseAuditEntityConfiguration<CountyParishHoldings>
{
    public override void Configure(EntityTypeBuilder<CountyParishHoldings> builder)
    {
        builder.ToTable(nameof(CountyParishHoldings).ToSnakeCase());

        builder.HasIndex(x => x.Identifier);

        builder.Property(x => x.Identifier)
            .HasColumnName(nameof(CountyParishHoldings.Identifier).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(x => x.ExpiredAt)
            .HasColumnName(nameof(CountyParishHoldings.ExpiredAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .IsRequired(false);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CountyParishHoldingsCreatedByUsers)
            .HasForeignKey(x => x.CreatedById);

        builder.HasOne(x => x.DeletedByUser)
            .WithMany(x => x.CountyParishHoldingsDeletedByUsers)
            .HasForeignKey(x => x.DeletedById);

        base.Configure(builder);
    }
}
