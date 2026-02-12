// <copyright file="MigrationTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Collections;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

[Collection(nameof(PostgreCollection))]
public class MigrationTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should execute all the database migrations")]
    public async Task ShouldMigrateAndAppliedMigrations()
    {
        var migrations = await Context.Database.GetAppliedMigrationsAsync(cancellationToken: TestContext.Current.CancellationToken);
        migrations.ShouldNotBeEmpty();
    }

    [Fact]
    [Description("All Base tables should exist")]
    public void ShouldAllBaseTablesExist()
    {
        // Check Base tables exist
        Context.Set<UserAccounts>().ShouldNotBeNull();
        Context.Set<Entities.Applications>().ShouldNotBeNull();
        Context.Set<Delegations>().ShouldNotBeNull();
        Context.Set<KrdsSyncLogs>().ShouldNotBeNull();
        Context.Set<Roles>().ShouldNotBeNull();
        Context.Set<CountyParishHoldings>().ShouldNotBeNull();
        Context.Set<DelegationsCountyParishHoldings>().ShouldNotBeNull();
        Context.Set<ApplicationRoles>().ShouldNotBeNull();
        Context.Set<ApplicationUserAccountHoldingAssignments>().ShouldNotBeNull();
        Context.Set<DelegationInvitations>().ShouldNotBeNull();
    }
}
