// <copyright file="CreateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.ApplicationsRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Applications;
using Defra.Identity.Repositories.Users;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

public class CreateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    private const string AdminEmailAddress = "test@test.com";

    [Fact]
    [Description("Should create a new application")]
    public async Task ShouldCreateApplication()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ApplicationsRepository>>();
        var repository = new ApplicationsRepository(Context, logger);

        var userLogger = Substitute.For<ILogger<UsersRepository>>();
        var userRepository = new UsersRepository(Context, userLogger);

        var adminUser = await userRepository.GetSingle(
            x => x.EmailAddress == AdminEmailAddress,
            TestContext.Current.CancellationToken);

        adminUser.ShouldNotBeNull("Seeded admin user was not found; check test data initialization.");

        var newApplication = new Applications
        {
            Name = "Test Application",
            ClientId = Guid.NewGuid(),
            TenantName = "Test Tenant",
            Description = "Test Description",
            CreatedById = adminUser.Id,
        };

        // Act
        var createdApplication = await repository.Create(newApplication, TestContext.Current.CancellationToken);

        // Assert
        createdApplication.ShouldSatisfyAllConditions(
            x => x.Name.ShouldBe("Test Application"),
            x => x.ClientId.ShouldBe(newApplication.ClientId),
            x => x.TenantName.ShouldBe("Test Tenant"),
            x => x.Description.ShouldBe("Test Description"),
            x => x.CreatedById.ShouldBe(adminUser.Id));

        logger.ReceivedWithAnyArgs().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
