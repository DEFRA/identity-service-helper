// <copyright file="BaseUpdateEntityConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Defra.Identity.Postgres.Database.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public abstract class BaseUpdateEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseUpdateEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(typeof(TEntity).Name.ToSnakeCase());

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName(nameof(Application.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .HasDefaultValueSql("uuid_generate_v4()")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName(nameof(BaseUpdateEntity.UpdatedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .ValueGeneratedNever()
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName(nameof(BaseUpdateEntity.CreatedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .HasDefaultValueSql(PostgreExtensions.Now)
            .ValueGeneratedOnAdd();
    }
}
