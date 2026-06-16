// <copyright file="CreateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.RoleRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Roles;
using Microsoft.Extensions.Logging;

public class CreateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should create a new role")]
    public async Task ShouldCreateRole()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<RoleRepository>();
        var repository = new RoleRepository(Context, ReadOnlyContext, logger);

        var newRole = new Roles() { Name = "New Role", Description = "New Role Description", };

        // Act
        var createdRole = await repository
            .Create(newRole, TestContext.Current.CancellationToken);

        // Assert
        createdRole.ShouldSatisfyAllConditions(
            x => x.Name.ShouldBe("New Role"),
            x => x.Description.ShouldBe("New Role Description"));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Creating role");
    }
}
