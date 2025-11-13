using Livestock.Auth.Database.Configuration.Base;
using Livestock.Auth.Database.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Auth.Database.Configuration;

internal class KrdsSyncLogConfiguration : BaseProcessingEntityConfiguration<KrdsSyncLog>
{
    public override void Configure(EntityTypeBuilder<KrdsSyncLog> builder)
    {
      
        builder.HasIndex(x => x.Upn);
        builder.HasIndex(x => x.ReceivedAt);
        
        builder.Property(x => x.CorrelationId)
            .HasColumnName(nameof(KrdsSyncLog.CorrelationId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier);
        
        builder.Property(x => x.Upn)
            .HasColumnName(nameof(KrdsSyncLog.Upn).ToSnakeCase())
            .HasColumnType(ColumnTypes.CiText);
        
        builder.Property(x => x.PayloadSha256)
            .HasColumnName(nameof(KrdsSyncLog.PayloadSha256).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text);
        
        builder.Property(x => x.SourceEndpoint)
            .HasColumnName(nameof(KrdsSyncLog.SourceEndpoint).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text);
        
        builder.Property(x => x.HttpStatus)
            .HasColumnName(nameof(KrdsSyncLog.HttpStatus).ToSnakeCase())
            .HasColumnType(ColumnTypes.Integer);
        
        builder.Property(x => x.ProcessedOk)
            .HasColumnName(nameof(KrdsSyncLog.ProcessedOk).ToSnakeCase())
            .HasColumnType(ColumnTypes.Boolean);
        
        builder.Property(x => x.Message)
            .HasColumnName(nameof(KrdsSyncLog.Message).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text);
        
        base.Configure(builder);
        
    }
}