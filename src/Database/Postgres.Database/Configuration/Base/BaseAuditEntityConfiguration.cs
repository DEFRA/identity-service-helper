// <copyright file="BaseAuditEntityConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration.Base;

using Defra.Identity.Postgres.Database.Entities.Base;

public abstract class BaseAuditEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseAuditEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(typeof(TEntity).Name.ToSnakeCase());

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName(nameof(BaseAuditEntity.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .HasDefaultValueSql(PostgreExtensions.UuidAlgorithm)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.DeletedById)
            .HasColumnName(nameof(BaseAuditEntity.DeletedById).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);

        builder.Property(x => x.DeletedAt)
            .HasColumnName(nameof(BaseAuditEntity.DeletedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .IsRequired(false)
            .ValueGeneratedNever();

        builder.Property(x => x.CreatedAt)
            .HasColumnName(nameof(BaseAuditEntity.CreatedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .HasDefaultValueSql(PostgreExtensions.Now)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CreatedById)
            .HasColumnName(nameof(BaseAuditEntity.CreatedById).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .HasColumnName(nameof(BaseAuditEntity.IsDeleted).ToSnakeCase())
            .HasColumnType(ColumnTypes.Boolean)
            .HasDefaultValue(false)
            .IsRequired();
    }
}
