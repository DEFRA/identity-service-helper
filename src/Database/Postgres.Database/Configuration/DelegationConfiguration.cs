// <copyright file="EnrolmentConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Configuration;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class DelegationConfiguration
    : BaseUpdateEntityConfiguration<Delegation>
{
    public override void Configure(EntityTypeBuilder<Delegation> builder)
    {
        builder.HasOne(x => x.CountyParishHolding)

        base.Configure(builder);
    }
}
