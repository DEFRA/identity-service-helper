// <copyright file="BaseTypeEntityConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration.Base;

using Defra.Identity.Postgres.Database.Entities.Base;

internal abstract class BaseTypeEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseTypeEntity
{
    protected virtual int NameMaxLength => 50;

    protected virtual int DescriptionMaxLength => 255;

    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(typeof(TEntity).Name.ToSnakeCase());
        builder.Property(x => x.Id)
            .HasColumnName(nameof(BaseTypeEntity.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .HasDefaultValueSql(PostgreExtensions.UuidAlgorithm);

        builder.Property(x => x.Name)
            .HasColumnName(nameof(BaseTypeEntity.Name).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(NameMaxLength);

        builder.Property(x => x.Description)
            .HasColumnName(nameof(BaseTypeEntity.Description).ToSnakeCase())
            .HasColumnType(ColumnTypes.Varchar)
            .HasMaxLength(DescriptionMaxLength);
    }
}
