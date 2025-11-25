// <copyright file="BaseProcessingEntityConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgre.Database.Configuration;

using Defra.Identity.Postgre.Database.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal abstract class BaseProcessingEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseProcessingEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(typeof(TEntity).Name.ToSnakeCase());
        builder.Property(x => x.Id)
            .HasColumnName(nameof(BaseProcessingEntity.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.Property(x => x.ReceivedAt)
            .HasColumnName(nameof(BaseProcessingEntity.ReceivedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .HasDefaultValueSql(PostgreExtensions.Now)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ProcessedAt)
            .HasColumnName(nameof(BaseProcessingEntity.ProcessedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);
    }
}
