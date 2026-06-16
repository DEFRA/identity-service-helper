// <copyright file="GetTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.RoleRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Roles;
using Microsoft.Extensions.Logging;

public class GetTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get a role")]
    public async Task ShouldGetSingleRole()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<RoleRepository>();
        var repository = new RoleRepository(Context, ReadOnlyContext, logger);

        var role = new Roles() { Name = "Single Test Role", Description = "Single Test Role Description" };

        await Context.Roles.AddAsync(role, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result =
            await repository.GetSingle(x => x.Name == "Single Test Role", TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();

        result.Name.ShouldBe("Single Test Role");
        result.Description.ShouldBe("Single Test Role Description");

        logger.VerifyLogContainsOne(LogLevel.Information, "Getting single role");
    }

    [Fact]
    [Description("Should get a list of roles")]
    public async Task ShouldGetListRoles()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<RoleRepository>();
        var repository = new RoleRepository(Context, ReadOnlyContext, logger);

        var roles = new List<Roles>
        {
            new() { Name = "List Test Role 1", Description = "List Test Role 1 Description" },
            new() { Name = "List Test Role 2", Description = "List Test Role 2 Description" },
        };

        await Context.Roles.AddRangeAsync(roles, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var results =
            (await repository.GetList(
                x => x.Name.Contains("List Test Role"),
                TestContext.Current.CancellationToken))
            .OrderBy(x => x.Name)
            .ToList();

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBe(2);

        results[0].ShouldSatisfyAllConditions(x =>
        {
            x.Name.ShouldBe("List Test Role 1");
            x.Description.ShouldBe("List Test Role 1 Description");
        });

        results[1].ShouldSatisfyAllConditions(x =>
        {
            x.Name.ShouldBe("List Test Role 2");
            x.Description.ShouldBe("List Test Role 2 Description");
        });

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Getting list of roles");
    }
}
