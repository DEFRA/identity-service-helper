// <copyright file="ExternalMessagingConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[ExcludeFromCodeCoverage]
internal class ExternalMessagingConfiguration : IEntityTypeConfiguration<ExternalMessaging>
{
    public void Configure(EntityTypeBuilder<ExternalMessaging> builder)
    {
        builder.ToTable(nameof(ExternalMessaging).ToSnakeCase());
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName(nameof(ExternalMessaging.Id).ToSnakeCase())
            .HasColumnType(ColumnTypes.Integer)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.MessageType)
            .HasColumnName(nameof(ExternalMessaging.MessageType).ToSnakeCase())
            .HasColumnType(ColumnTypes.SmallInt)
            .IsRequired();

        builder.Property(x => x.MessageRecipient)
            .HasColumnName(nameof(ExternalMessaging.MessageRecipient).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .IsRequired();

        builder.Property(x => x.TemplateId)
            .HasColumnName(nameof(ExternalMessaging.TemplateId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.Property(x => x.NotifyId)
            .HasColumnName(nameof(ExternalMessaging.NotifyId).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired();

        builder.Property(x => x.RequestPayload)
            .HasColumnName(nameof(ExternalMessaging.RequestPayload).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .IsRequired(false);

        builder.Property(x => x.SentAt)
            .HasColumnName(nameof(ExternalMessaging.SentAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .IsRequired(false);

        builder.Property(x => x.ResponseCode)
            .HasColumnName(nameof(ExternalMessaging.ResponseCode).ToSnakeCase())
            .HasColumnType(ColumnTypes.Integer)
            .IsRequired(false);

        builder.Property(x => x.ResponseMessage)
            .HasColumnName(nameof(ExternalMessaging.ResponseMessage).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .IsRequired(false);

        builder.Property(x => x.ExceptionMessage)
            .HasColumnName(nameof(ExternalMessaging.ExceptionMessage).ToSnakeCase())
            .HasColumnType(ColumnTypes.Text)
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName(nameof(ExternalMessaging.CreatedAt).ToSnakeCase())
            .HasColumnType(ColumnTypes.Timestamp)
            .IsRequired();

        builder.Property(x => x.CreatedById)
            .HasColumnName(nameof(ExternalMessaging.CreatedById).ToSnakeCase())
            .HasColumnType(ColumnTypes.UniqueIdentifier)
            .IsRequired(false);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.ExternalMessagingCreatedByUsers)
            .HasForeignKey(x => x.CreatedById)
            .HasConstraintName("external_messaging_user_accounts_id_fk");
    }
}
