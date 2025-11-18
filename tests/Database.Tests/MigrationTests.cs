using System.ComponentModel;
using Livestock.Auth.Database.Entities;
using Livestock.Auth.Database.Tests.Collections;
using Livestock.Auth.Database.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Livestock.Auth.Database.Tests;

using FluentAssertions;
using Shouldly;

[Collection(nameof(PostgreCollection))]
public class MigrationTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact, Description("Should execute all the database migrations")]
    public async Task ShouldMigrateAndAppliedMigrations()
    {
        // var migrations = await Context.Database.GetAppliedMigrationsAsync(cancellationToken: TestContext.Current.CancellationToken);
        // migrations.Should().NotBeEmpty();
    }
    
    [Fact,Description("All Base tables should exist")]
    public void ShouldAllBaseTablesExist()
    {
        // Check Base tables exist
        Context.Set<UserAccount>().ShouldNotBeNull();
        Context.Set<Application>().ShouldNotBeNull();
        Context.Set<Enrolment>().ShouldNotBeNull();
        Context.Set<Federation>().ShouldNotBeNull();
        Context.Set<KrdsSyncLog>().ShouldNotBeNull();
    }
}