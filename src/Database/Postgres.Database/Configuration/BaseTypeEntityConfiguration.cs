// <copyright file="BaseTypeEntityConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Defra.Identity.Postgres.Database.Entities.Base;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

internal abstract class BaseTypeEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseTypeEntity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(typeof(TEntity).Name.ToSnakeCase());
        builder.Property(x => x.Id)
            .HasColumnName(nameof(BaseTypeEntity.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.SmallInt)
            .UseIdentityAlwaysColumn();

        builder.Property(x => x.Name)
            .HasColumnName(nameof(BaseTypeEntity.Name).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasColumnName(nameof(BaseTypeEntity.Description).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(255);
    }
}
