// <copyright file="ApplicationServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Applications;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Applications;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Requests.Applications.Commands.Create;
using Defra.Identity.Requests.Applications.Commands.Update;
using Defra.Identity.Requests.Applications.Queries;
using Defra.Identity.Services.Applications;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

public class ApplicationServiceTests
{
    private readonly IApplicationsRepository repository = Substitute.For<IApplicationsRepository>();
    private readonly ILogger<ApplicationService> logger = Substitute.For<ILogger<ApplicationService>>();
    private readonly ApplicationService applicationService;

    public ApplicationServiceTests()
    {
        applicationService = new ApplicationService(repository, logger);
    }

    [Fact]
    public async Task GetAll_ReturnsApplications()
    {
        // Arrange
        var request = new GetApplications();
        var applicationEntities = new List<Applications>
        {
            new Applications { Id = Guid.NewGuid(), Name = "App 1", ClientId = Guid.NewGuid(), TenantName = "Tenant 1" },
            new Applications { Id = Guid.NewGuid(), Name = "App 2", ClientId = Guid.NewGuid(), TenantName = "Tenant 2" },
        };

        repository.GetList(Arg.Any<Expression<Func<Applications, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(applicationEntities);

        // Act
        var result = await applicationService.GetAll(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result[0].Name.ShouldBe("App 1");
        result[1].Name.ShouldBe("App 2");
    }

    [Fact]
    public async Task Get_ApplicationExists_ReturnsApplication()
    {
        // Arrange
        var appId = Guid.NewGuid();
        var request = new GetApplicationById { Id = appId };
        var applicationEntity = new Applications
        {
            Id = appId,
            Name = "Test App",
            ClientId = Guid.NewGuid(),
            TenantName = "Test Tenant",
        };

        repository.GetSingle(Arg.Any<Expression<Func<Applications, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(applicationEntity);

        // Act
        var result = await applicationService.Get(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(appId);
        result.Name.ShouldBe("Test App");
    }

    [Fact]
    public async Task Get_ApplicationDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var request = new GetApplicationById { Id = Guid.NewGuid() };
        repository.GetSingle(Arg.Any<Expression<Func<Applications, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Applications)null!);

        // Act
        Func<Task> act = async () => await applicationService.Get(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Create_CallsRepository()
    {
        // Arrange
        var request = new CreateApplication
        {
            Name = "New App",
            ClientId = Guid.NewGuid(),
            TenantName = "New Tenant",
            OperatorId = Guid.NewGuid(),
        };

        var createdEntity = new Applications
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ClientId = request.ClientId,
            TenantName = request.TenantName,
        };

        repository.Create(Arg.Any<Applications>(), Arg.Any<CancellationToken>())
            .Returns(createdEntity);

        // Act
        var result = await applicationService.Create(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe(request.Name);
        await repository.Received(1).Create(Arg.Any<Applications>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_ApplicationExists_UpdatesAndReturnsApplication()
    {
        // Arrange
        var appId = Guid.NewGuid();
        var request = new UpdateApplication
        {
            Id = appId,
            Name = "Updated App",
            ClientId = Guid.NewGuid(),
            TenantName = "Updated Tenant",
        };

        var existingEntity = new Applications
        {
            Id = appId,
            Name = "Old App",
            ClientId = Guid.NewGuid(),
            TenantName = "Old Tenant",
        };

        repository.GetSingle(Arg.Any<Expression<Func<Applications, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(existingEntity);

        repository.Update(Arg.Any<Applications>(), Arg.Any<CancellationToken>())
            .Returns(x => (Applications)x[0]);

        // Act
        var result = await applicationService.Update(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Updated App");
        await repository.Received(1).Update(Arg.Any<Applications>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_CallsRepository()
    {
        // Arrange
        var appId = Guid.NewGuid();
        var operatorId = Guid.NewGuid();
        repository.Delete(Arg.Any<Expression<Func<Applications, bool>>>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await applicationService.Delete(appId, operatorId, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeTrue();
        await repository.Received(1).Delete(Arg.Any<Expression<Func<Applications, bool>>>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
