using Livestock.Auth.Database.Configuration.Base;
using Livestock.Auth.Database.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livestock.Auth.Database.Configuration;

internal class EnrolmentConfiguration : BaseUpdateEntityConfiguration<Enrolment>
{
    public override void Configure(EntityTypeBuilder<Enrolment> builder)
    {
        builder.HasIndex(x => new  { B2cObjectId = x.UserAccountId, x.Role}).IsUnique();
        
        builder.Property(x => x.UserAccountId)
            .HasColumnName(nameof(Enrolment.UserAccountId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();
        
        builder.HasOne(u => u.UserAccount)
            .WithMany(o => o.Enrolments)
            .HasForeignKey(u => u.UserAccountId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.ApplicationId)
            .HasColumnName(nameof(Enrolment.ApplicationId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();
        
        builder.Property(x => x.CphId)
            .HasColumnName(nameof(Enrolment.CphId).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .IsRequired();
        
        builder.Property(x => x.Role)
            .HasColumnName(nameof(Enrolment.Role).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text);
        
        builder.Property(x => x.Scope)
            .HasColumnName(nameof(Enrolment.Scope).ToSnakeCase())
            .HasColumnType(ColumnTypes.JsonB);
        
        builder.Property(x => x.Status)
            .HasColumnName(nameof(Enrolment.Status).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .HasDefaultValue("active");
        
        builder.Property(x => x.EnrolledAt)
            .HasColumnName(nameof(Enrolment.EnrolledAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .HasDefaultValueSql("now()");

        
        builder.Property(x => x.ExpiresAt)
            .HasColumnName(nameof(Enrolment.ExpiresAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp);
        
        
        base.Configure(builder);
    }
}