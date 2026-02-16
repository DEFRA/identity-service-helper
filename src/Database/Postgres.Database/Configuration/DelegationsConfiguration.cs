// <copyright file="DelegationsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Defra.Identity.Postgres.Database.Configuration.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class DelegationsConfiguration
    : BaseAuditEntityConfiguration<Delegations>
{
    public override void Configure(EntityTypeBuilder<Delegations> builder)
    {
        builder.Property(x => x.ApplicationId)
            .HasColumnName(nameof(Delegations.ApplicationId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.HasOne(x => x.Applications)
            .WithMany(x => x.Delegations)
            .HasForeignKey(x => x.ApplicationId);

        builder.Property(x => x.UserId)
            .HasColumnName(nameof(Delegations.UserId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.HasOne(x => x.UserAccount)
            .WithMany(x => x.Delegations)
            .HasForeignKey(x => x.UserId);

        base.Configure(builder);
    }
}
