// <copyright file="UpdateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.ApplicationsRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Applications;
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

        var application = new Applications
        {
            Id = Guid.NewGuid(),
            Name = "Original Name",
            ClientId = Guid.NewGuid(),
            TenantName = "Test Tenant",
            CreatedById = adminUser.Id,
        };

        await Context.Applications.AddAsync(application, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        application.Name = "Updated Name";

        // Act
        var updatedApplication = await repository.Update(application, TestContext.Current.CancellationToken);

        // Assert
        updatedApplication.Name.ShouldBe("Updated Name");
        var dbApp = await Context.Applications.FindAsync([application.Id], TestContext.Current.CancellationToken);
        dbApp.ShouldNotBeNull();
        dbApp.Name.ShouldBe("Updated Name");

        logger.VerifyLogContainsOne(LogLevel.Information, $"Updating application with id {application.Id}");
    }
}
