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
    private const string EmailAddress = "test@test.com";

    [Fact]
    [Description("Should create a new application")]
    public async Task ShouldCreateApplication()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<ApplicationsRepository>();
        var repository = new ApplicationsRepository(Context, ReadOnlyContext, logger);

        var userLogger = DefraLoggerExtensions.CreateNSubstituteLogger<UserRepository>();
        var userRepository = new UserRepository(Context, ReadOnlyContext, userLogger);

        var adminUser = await userRepository.GetSingle(
            x => x.EmailAddress == EmailAddress,
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
        var createdApplication = await repository
            .Create(newApplication, TestContext.Current.CancellationToken);

        // Assert
        createdApplication.ShouldSatisfyAllConditions(
            x => x.Name.ShouldBe("Test Application"),
            x => x.ClientId.ShouldBe(newApplication.ClientId),
            x => x.TenantName.ShouldBe("Test Tenant"),
            x => x.Description.ShouldBe("Test Description"),
            x => x.CreatedById.ShouldBe(adminUser.Id));

        logger.VerifyLogContainsOne(LogLevel.Information, "Creating application");
    }
}
