// <copyright file="KrdsSyncLogsConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Defra.Identity.Postgres.Database.Configuration.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class KrdsSyncLogsConfiguration : BaseProcessingEntityConfiguration<KrdsSyncLogs>
{
    public override void Configure(EntityTypeBuilder<KrdsSyncLogs> builder)
    {
        builder.HasIndex(x => x.ReceivedAt);

        builder.Property(x => x.CorrelationId)
            .HasColumnName(nameof(KrdsSyncLogs.CorrelationId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.Property(x => x.SourceEndpoint)
            .HasColumnName(nameof(KrdsSyncLogs.SourceEndpoint).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text);

        builder.Property(x => x.HttpStatus)
            .HasColumnName(nameof(KrdsSyncLogs.HttpStatus).ToSnakeCase())
            .HasColumnType(ColumnTypes.Integer);

        builder.Property(x => x.ProcessedOk)
            .HasColumnName(nameof(KrdsSyncLogs.ProcessedOk).ToSnakeCase())
            .HasColumnType(ColumnTypes.Boolean);

        builder.Property(x => x.Message)
            .HasColumnName(nameof(KrdsSyncLogs.Message).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text);

        base.Configure(builder);
    }
}
