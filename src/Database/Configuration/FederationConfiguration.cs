using Livestock.Auth.Database.Configuration.Base;
using Livestock.Auth.Database.Entities;

namespace Livestock.Auth.Database.Configuration;

internal class FederationConfiguration : BaseUpdateEntityConfiguration<Federation>
{
    public override void Configure(EntityTypeBuilder<Federation> builder)
    {
        
        
        builder.Property(x => x.UserAccountId)
            .HasColumnName(nameof(Federation.UserAccountId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);
        
        builder.Property(x => x.TenantName)
            .HasColumnName(nameof(Federation.TenantName).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .IsRequired();
        
        builder.Property(x => x.ObjectId)
            .HasColumnName(nameof(Federation.ObjectId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            
            .IsRequired();
        
        builder.Property(x => x.TrustLevel)
            .HasColumnName(nameof(Federation.TrustLevel).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .HasDefaultValue("standard")
            .IsRequired();
        
        builder.Property(x => x.SyncStatus)
            .HasColumnName(nameof(Federation.SyncStatus).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .HasDefaultValue("linked")
            .IsRequired();
        
        builder.Property(x => x.LastSyncedAt)
            .HasColumnName(nameof(Federation.LastSyncedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);
        
        
        builder.HasOne(u => u.UserAccount)
            .WithMany(o => o.Federations)
            .HasForeignKey(u => u.UserAccountId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasIndex(x => new{ B2cObjectId = x.ObjectId, B2cTenant = x.TenantName});
        
        base.Configure(builder);
        
    }
}