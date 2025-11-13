using Livestock.Auth.Database.Configuration.Base;
using Livestock.Auth.Database.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Auth.Database.Configuration;

internal class UserAccountConfiguration : BaseUpdateEntityConfiguration<UserAccount>
{
    public override void Configure(EntityTypeBuilder<UserAccount> builder)
    {
       builder.HasIndex(x => x.Upn);
       
       builder.Property(x => x.Upn)
           .HasColumnName(nameof(UserAccount.Upn).ToSnakeCase())
           .HasColumnType(ColumnTypes.CiText);

       builder.Property(x => x.DisplayName)
           .HasColumnName(nameof(UserAccount.DisplayName).ToSnakeCase())
           .HasColumnType(ColumnTypes.Varchar)
           .HasMaxLength(256);
       
       builder.Property(x => x.AccountEnabled)
           .HasColumnName(nameof(UserAccount.AccountEnabled).ToSnakeCase())
           .HasColumnType(ColumnTypes.Boolean)
           .HasDefaultValue(true);
       
       base.Configure(builder);

    }
}