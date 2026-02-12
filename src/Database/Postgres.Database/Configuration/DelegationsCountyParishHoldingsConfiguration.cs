// <copyright file="DelegationsCountyParishHoldingsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Defra.Identity.Postgres.Database.Configuration.Base;

public class DelegationsCountyParishHoldingsConfiguration : BaseAuditEntityConfiguration<DelegationsCountyParishHoldings>
{
    public override void Configure(EntityTypeBuilder<DelegationsCountyParishHoldings> builder)
    {
        builder.Property(x => x.DelegationId)
            .HasColumnName(nameof(DelegationsCountyParishHoldings.DelegationId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.Delegation)
            .WithMany(x => x.DelegationsCountyParishHoldings)
            .HasForeignKey(x => x.DelegationId);

        builder.Property(x => x.CountyParishHoldingId)
            .HasColumnName(nameof(DelegationsCountyParishHoldings.CountyParishHoldingId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.CountyParishHolding)
            .WithMany(x => x.DelegationsCountyParishHoldings)
            .HasForeignKey(x => x.CountyParishHoldingId);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.DelegationsCountyParishHoldingsCreatedByUsers)
            .HasForeignKey(x => x.CreatedById);

        builder.HasOne(x => x.DeletedByUser)
            .WithMany(x => x.DelegationsCountyParishHoldingsDeletedByUsers)
            .HasForeignKey(x => x.DeletedById);

        base.Configure(builder);
    }
}
