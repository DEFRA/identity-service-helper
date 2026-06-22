// <copyright file="CreateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.ApplicationsRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Applications;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Test.Utilities.Assertions;
using Microsoft.Extensions.Logging;
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

        var clientId = Guid.NewGuid();

        adminUser.ShouldNotBeNull("Seeded admin user was not found; check test data initialization.");

        var newApplication = new Applications
        {
            Name = "Test Application",
            ClientId = clientId,
            TenantName = "Test Tenant",
            Description = "Test Description",
            Scopes = "scope1;scope2",
            RedirectUris = "http://redirect/1;http://redirect/2",
            Secret = "secret",
            CreatedById = adminUser.Id,
            CreatedAt = DateTime.UtcNow,
            DeletedById = adminUser.Id,
            DeletedAt = DateTime.UtcNow,
        };

        // Act
        var createdApplication = await repository
            .Create(newApplication, TestContext.Current.CancellationToken);

        // Assert
        createdApplication.ShouldSatisfyAllConditions(
            x => x.Name.ShouldBe("Test Application"),
            x => x.ClientId.ShouldBe(clientId),
            x => x.TenantName.ShouldBe("Test Tenant"),
            x => x.Description.ShouldBe("Test Description"),
            x => x.Scopes.ShouldBe("scope1;scope2"),
            x => x.RedirectUris.ShouldBe("http://redirect/1;http://redirect/2"),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBeCloseToUtcNow(),
            x => x.DeletedById.ShouldBe(adminUser.Id),
            x => x.DeletedAt.ShouldNotBeNull(),
            x => x.DeletedAt!.Value.ShouldBeCloseToUtcNow());

        logger.VerifyLogContainsOne(LogLevel.Information, "Creating application");
    }
}
