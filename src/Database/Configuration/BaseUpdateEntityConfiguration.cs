// <copyright file="BaseUpdateEntityConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Database.Configuration.Base;

using Defra.Identity.Database.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

public abstract class BaseUpdateEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseUpdateEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(typeof(TEntity).Name.ToSnakeCase());
        builder.Property(x => x.Id)
            .HasColumnName(nameof(BaseUpdateEntity.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName(nameof(BaseUpdateEntity.UpdatedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);

        builder.Property(x => x.CreatedAt)
            .HasColumnName(nameof(BaseUpdateEntity.CreatedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .HasDefaultValueSql(PostgreExtensions.Now)
            .ValueGeneratedOnAdd();
    }
}
