using Livestock.Auth.Database.Entities;
using Livestock.Auth.Database.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Auth.Database.Configuration.Base;

public abstract class BaseUpdateEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity: BaseUpdateEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(typeof (TEntity).Name.ToSnakeCase());
        builder.Property(x => x.Id)
            .HasColumnName(nameof(BaseUpdateEntity.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);
        
        builder.Property(x => x.UpdatedAt)
            .HasColumnName(nameof(BaseUpdateEntity.UpdatedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .HasDefaultValueSql("now()");
       
        builder.Property(x => x.CreatedAt)
            .HasColumnName(nameof(BaseUpdateEntity.CreatedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .HasDefaultValueSql("now()");
        
    }
}