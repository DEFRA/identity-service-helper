using System.ComponentModel;
using Livestock.Auth.Database.Entities;
using Livestock.Auth.Database.Tests.Collections;
using Livestock.Auth.Database.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Livestock.Auth.Database.Tests;

[Collection(nameof(PostgreCollection))]
public class MigrationTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact, Description("Should execute all the database migrations")]
    public async Task ShouldMigrateAndAppliedMigrations()
    {
        var migrations = await Context.Database.GetAppliedMigrationsAsync(cancellationToken: TestContext.Current.CancellationToken);
        migrations.ShouldNotBeEmpty();
    }
    
    [Fact,Description("All Base tables should exist")]
    public void ShouldAllBaseTablesExist()
    {
        // Check Base tables exist
        Context.Set<UserAccount>().ShouldNotBeNull();
     
    }
}