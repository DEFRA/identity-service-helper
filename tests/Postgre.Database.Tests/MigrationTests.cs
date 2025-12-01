// <copyright file="MigrationTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgre.Database.Tests;

using System.ComponentModel;
using Defra.Identity.Postgre.Database.Entities;
using Defra.Identity.Postgre.Database.Tests.Collections;
using Defra.Identity.Postgre.Database.Tests.Fixtures;
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
        Context.Set<Entities.Applications>().ShouldNotBeNull();
        Context.Set<Application>().ShouldNotBeNull();
        Context.Set<Enrolment>().ShouldNotBeNull();
        Context.Set<Federation>().ShouldNotBeNull();
        Context.Set<KrdsSyncLog>().ShouldNotBeNull();
    }
}
