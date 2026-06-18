// <copyright file="GetTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.ApplicationsRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Applications;
using Defra.Identity.Test.Utilities.Assertions;
using Microsoft.Extensions.Logging;
using Shouldly;

public class GetTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get a single application")]
    public async Task ShouldGetSingleApplication()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<ApplicationsRepository>();
        var repository = new ApplicationsRepository(Context, ReadOnlyContext, logger);

        var adminUser = Context.UserAccounts.First();

        var clientId = Guid.NewGuid();

        adminUser.ShouldNotBeNull("Seeded admin user was not found; check test data initialization.");

        var application = new Applications
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

        await Context.Applications.AddAsync(application, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result =
            await repository.GetSingle(x => x.ClientId == clientId, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();

        result.ShouldSatisfyAllConditions(
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

        logger.VerifyLogContainsOne(LogLevel.Information, "Getting single application");
    }

    [Fact]
    [Description("Should get a list of applications")]
    public async Task ShouldGetListApplications()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<ApplicationsRepository>();
        var repository = new ApplicationsRepository(Context, ReadOnlyContext, logger);

        var adminUser = Context.UserAccounts.First();

        var clientId1 = Guid.NewGuid();
        var clientId2 = Guid.NewGuid();

        var apps = new List<Applications>
        {
            new()
            {
                Name = "Test Application 1",
                ClientId = clientId1,
                TenantName = "Test Tenant 1",
                Description = "Test Description 1",
                Scopes = "scope1;scope2",
                RedirectUris = "http://redirect/1;http://redirect/2",
                Secret = "secret1",
                CreatedById = adminUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(2),
                DeletedById = adminUser.Id,
                DeletedAt = DateTime.UtcNow,
            },
            new()
            {
                Name = "Test Application 2",
                ClientId = clientId2,
                TenantName = "Test Tenant 2",
                Description = "Test Description 2",
                Scopes = "scope3;scope4",
                RedirectUris = "http://redirect/3;http://redirect/4",
                Secret = "secret2",
                CreatedById = adminUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(2),
                DeletedById = adminUser.Id,
                DeletedAt = DateTime.UtcNow,
            },
        };
        await Context.Applications.AddRangeAsync(apps, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var results =
            (await repository.GetList(
                x => x.CreatedAt > DateTime.UtcNow,
                TestContext.Current.CancellationToken))
            .OrderBy(x => x.Name)
            .ToList();

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBe(2);

        results[0].ShouldSatisfyAllConditions(
            x => x.Name.ShouldBe("Test Application 1"),
            x => x.ClientId.ShouldBe(clientId1),
            x => x.TenantName.ShouldBe("Test Tenant 1"),
            x => x.Description.ShouldBe("Test Description 1"),
            x => x.Scopes.ShouldBe("scope1;scope2"),
            x => x.RedirectUris.ShouldBe("http://redirect/1;http://redirect/2"),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBeCloseToUtcNowAddDays(2),
            x => x.DeletedById.ShouldBe(adminUser.Id),
            x => x.DeletedAt.ShouldNotBeNull(),
            x => x.DeletedAt!.Value.ShouldBeCloseToUtcNow());

        results[1].ShouldSatisfyAllConditions(
            x => x.Name.ShouldBe("Test Application 2"),
            x => x.ClientId.ShouldBe(clientId2),
            x => x.TenantName.ShouldBe("Test Tenant 2"),
            x => x.Description.ShouldBe("Test Description 2"),
            x => x.Scopes.ShouldBe("scope3;scope4"),
            x => x.RedirectUris.ShouldBe("http://redirect/3;http://redirect/4"),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBeCloseToUtcNowAddDays(2),
            x => x.DeletedById.ShouldBe(adminUser.Id),
            x => x.DeletedAt.ShouldNotBeNull(),
            x => x.DeletedAt!.Value.ShouldBeCloseToUtcNow());

        logger.VerifyLogContainsOne(LogLevel.Information, "Getting list of applications");
    }
}
