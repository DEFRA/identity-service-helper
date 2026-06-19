// <copyright file="UpdateTests.cs" company="Defra">
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

public class UpdateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should update an application")]
    public async Task ShouldUpdateApplication()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<ApplicationsRepository>();
        var repository = new ApplicationsRepository(Context, ReadOnlyContext, logger);

        var adminUser = Context.UserAccounts.First();

        var clientId = Guid.NewGuid();
        var clientIdChanged = Guid.NewGuid();

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
        };

        await Context.Applications.AddAsync(application, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        application.Name = "Test Application 2";
        application.ClientId = clientIdChanged;
        application.TenantName = "Test Tenant 2";
        application.Description = "Test Description 2";
        application.Scopes = "scope3;scope4";
        application.RedirectUris = "http://redirect/3;http://redirect/4";
        application.Secret = "secret2";
        application.DeletedById = adminUser.Id;
        application.DeletedAt = DateTime.UtcNow;

        // Act
        var updatedApplication = await repository.Update(application, TestContext.Current.CancellationToken);

        // Assert
        updatedApplication.ShouldSatisfyAllConditions(
            x => x.Name.ShouldBe("Test Application 2"),
            x => x.ClientId.ShouldBe(clientIdChanged),
            x => x.TenantName.ShouldBe("Test Tenant 2"),
            x => x.Description.ShouldBe("Test Description 2"),
            x => x.Scopes.ShouldBe("scope3;scope4"),
            x => x.RedirectUris.ShouldBe("http://redirect/3;http://redirect/4"),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBeCloseToUtcNow(),
            x => x.DeletedById.ShouldBe(adminUser.Id),
            x => x.DeletedAt.ShouldNotBeNull(),
            x => x.DeletedAt!.Value.ShouldBeCloseToUtcNow());

        logger.VerifyLogContainsOne(LogLevel.Information, $"Updating application with id {application.Id}");
    }
}
