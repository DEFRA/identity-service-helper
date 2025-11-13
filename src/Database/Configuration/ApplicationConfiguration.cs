using Livestock.Auth.Database.Configuration.Base;
using Livestock.Auth.Database.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Auth.Database.Configuration;

internal class ApplicationConfiguration :BaseUpdateEntityConfiguration<Application>
{
    public override void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.Property(x => x.Name)
            .HasColumnName(nameof(Application.Name).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .HasComment("Human readable name for the application e.g Keeper Portal")
            .IsRequired();
        
        builder.Property(x => x.ClientId)
            .HasColumnName(nameof(Application.ClientId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .HasComment("Azure AD B2C application Client ID")
            .IsRequired();
        
        builder.Property(x => x. TenantName)
            .HasColumnName(nameof(Application.TenantName).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .HasComment("Azure AD B2C tenant name e.g defra.onmicrosoft.com")
            .IsRequired();
        
        builder.Property(x => x.Description)
            .HasColumnName(nameof(Application.Description).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text);
        
        builder.Property(x => x.Status)
            .HasColumnName(nameof(Application.Status).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .HasDefaultValue("active")
            .HasComment("active/inactive/deprecated")
            .IsRequired();
        
        base.Configure(builder);
    }
}