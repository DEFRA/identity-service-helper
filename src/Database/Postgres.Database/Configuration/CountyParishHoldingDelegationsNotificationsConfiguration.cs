// <copyright file="CountyParishHoldingDelegationsNotificationsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class CountyParishHoldingDelegationsNotificationsConfiguration
    : IEntityTypeConfiguration<CountyParishHoldingDelegationsNotifications>
{
    public void Configure(EntityTypeBuilder<CountyParishHoldingDelegationsNotifications> builder)
    {
        builder.ToTable(nameof(CountyParishHoldingDelegationsNotifications).ToSnakeCase());
        builder.HasKey(x => new { x.MessageId, x.DelegationId });

        builder.Property(x => x.DelegationId)
            .HasColumnName(nameof(CountyParishHoldingDelegationsNotifications.DelegationId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.Property(x => x.MessageId)
            .HasColumnName(nameof(CountyParishHoldingDelegationsNotifications.MessageId).ToSnakeCase())
            .HasColumnType(ColumnTypes.Integer)
            .IsRequired();

        builder.HasOne(x => x.Delegation)
            .WithMany(x => x.CountyParishHoldingDelegationsNotifications)
            .HasForeignKey(x => x.DelegationId)
            .HasConstraintName("cph_delegations_notifications_cph_delegation_id_fk");

        builder.HasOne(x => x.Message)
            .WithMany(x => x.CountyParishHoldingDelegationsNotifications)
            .HasForeignKey(x => x.MessageId)
            .HasConstraintName("cph_delegations_notifications_external_messaging_id_fk");
    }
}
