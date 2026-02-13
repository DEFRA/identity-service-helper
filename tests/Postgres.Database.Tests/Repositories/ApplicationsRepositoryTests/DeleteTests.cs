// <copyright file="DeleteTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.ApplicationsRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Applications;
using Defra.Identity.Repositories.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

public class DeleteTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should soft delete an application")]
    public async Task ShouldDeleteApplication()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ApplicationsRepository>>();
        var repository = new ApplicationsRepository(Context, logger);

        var adminUser = Context.UserAccounts.First();

        var application = new Applications
        {
            Name = "To Be Deleted",
            ClientId = Guid.NewGuid(),
            TenantName = "Test Tenant",
            CreatedById = adminUser.Id,
        };
        await Context.Applications.AddAsync(application, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var operatorId = adminUser.Id;

        // Act
        var result = await repository.Delete(x => x.Id == application.Id, operatorId, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeTrue();
        var deletedApp = await Context.Applications.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == application.Id, TestContext.Current.CancellationToken);
        deletedApp.ShouldNotBeNull();
        deletedApp.DeletedById.ShouldBe(operatorId);
        deletedApp.DeletedAt.ShouldNotBeNull();
    }

    [Fact]
    [Description("Should throw NotFoundException when application does not exist")]
    public async Task ShouldThrowNotFoundExceptionWhenAppDoesNotExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ApplicationsRepository>>();
        var repository = new ApplicationsRepository(Context, logger);
        var adminUser = Context.UserAccounts.First();
        var operatorId = adminUser.Id;

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(async () =>
            await repository.Delete(x => x.Id == Guid.NewGuid(), operatorId, TestContext.Current.CancellationToken));
    }
}
