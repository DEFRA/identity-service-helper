// <copyright file="DelegationsServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Applications;
using Defra.Identity.Repositories.Delegates;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Requests.Delegations.Commands.Create;
using Defra.Identity.Requests.Delegations.Commands.Update;
using Defra.Identity.Requests.Delegations.Queries;
using Defra.Identity.Responses.Delegations;
using Defra.Identity.Services.Delegations;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

public class DelegationsServiceTests
{
    private readonly IDelegatesRepository repository = Substitute.For<IDelegatesRepository>();
    private readonly IUsersRepository usersRepository = Substitute.For<IUsersRepository>();
    private readonly IApplicationsRepository applicationsRepository = Substitute.For<IApplicationsRepository>();
    private readonly ILogger<DelegationsService> logger = Substitute.For<ILogger<DelegationsService>>();
    private readonly DelegationsService service;

    public DelegationsServiceTests()
    {
        service = new DelegationsService(repository, usersRepository, applicationsRepository, logger);
    }

    [Fact]
    public async Task GetAll_ReturnsDelegations()
    {
        // Arrange
        var request = new GetDelegations();
        var entities = new List<Delegations>
        {
            new Delegations { Id = Guid.NewGuid(), ApplicationId = Guid.NewGuid(), UserId = Guid.NewGuid() },
            new Delegations { Id = Guid.NewGuid(), ApplicationId = Guid.NewGuid(), UserId = Guid.NewGuid() },
        };

        repository.GetList(Arg.Any<Expression<Func<Delegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(entities);

        // Act
        var result = await service.GetAll(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Get_DelegationExists_ReturnsDelegation()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new GetDelegationById { Id = id };
        var entity = new Delegations { Id = id, ApplicationId = Guid.NewGuid(), UserId = Guid.NewGuid() };

        repository.GetSingle(Arg.Any<Expression<Func<Delegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(entity);

        // Act
        var result = await service.Get(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(id);
    }

    [Fact]
    public async Task Get_DelegationDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var request = new GetDelegationById { Id = Guid.NewGuid() };
        repository.GetSingle(Arg.Any<Expression<Func<Delegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Delegations)null!);

        // Act
        Func<Task> act = async () => await service.Get(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Create_CallsRepository_WhenReferencesExist()
    {
        // Arrange
        var request = new CreateDelegation
        {
            ApplicationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid(),
        };

        applicationsRepository.GetSingle(Arg.Any<Expression<Func<Applications, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new Applications { Id = request.ApplicationId, Name = "Test App", ClientId = Guid.NewGuid(), TenantName = "Test Tenant" });
        usersRepository.GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new UserAccounts { Id = request.UserId });

        var created = new Delegations { Id = Guid.NewGuid(), ApplicationId = request.ApplicationId, UserId = request.UserId };

        repository.Create(Arg.Any<Delegations>(), Arg.Any<CancellationToken>())
            .Returns(created);

        // Act
        var result = await service.Create(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.ApplicationId.ShouldBe(request.ApplicationId);
        await repository.Received(1).Create(Arg.Any<Delegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_ThrowsNotFoundException_WhenApplicationDoesNotExist()
    {
        // Arrange
        var request = new CreateDelegation
        {
            ApplicationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid(),
        };

        applicationsRepository.GetSingle(Arg.Any<Expression<Func<Applications, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Applications)null!);

        // Act
        Func<Task> act = async () => await service.Create(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        await repository.DidNotReceive().Create(Arg.Any<Delegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new CreateDelegation
        {
            ApplicationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid(),
        };

        applicationsRepository.GetSingle(Arg.Any<Expression<Func<Applications, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new Applications { Id = request.ApplicationId, Name = "Test App", ClientId = Guid.NewGuid(), TenantName = "Test Tenant" });
        usersRepository.GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((UserAccounts)null!);

        // Act
        Func<Task> act = async () => await service.Create(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        await repository.DidNotReceive().Create(Arg.Any<Delegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_DelegationExists_UpdatesAndReturnsDelegation_WhenReferencesExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateDelegation { Id = id, ApplicationId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        var existing = new Delegations { Id = id, ApplicationId = Guid.NewGuid(), UserId = Guid.NewGuid() };

        repository.GetSingle(Arg.Any<Expression<Func<Delegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(existing);
        applicationsRepository.GetSingle(Arg.Any<Expression<Func<Applications, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new Applications { Id = request.ApplicationId, Name = "Test App", ClientId = Guid.NewGuid(), TenantName = "Test Tenant" });
        usersRepository.GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new UserAccounts { Id = request.UserId });

        repository.Update(Arg.Any<Delegations>(), Arg.Any<CancellationToken>())
            .Returns(x => (Delegations)x[0]);

        // Act
        var result = await service.Update(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(id);
        result.ApplicationId.ShouldBe(request.ApplicationId);
        result.UserId.ShouldBe(request.UserId);
        await repository.Received(1).Update(Arg.Any<Delegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_ThrowsNotFoundException_WhenApplicationDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateDelegation { Id = id, ApplicationId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        var existing = new Delegations { Id = id, ApplicationId = Guid.NewGuid(), UserId = Guid.NewGuid() };

        repository.GetSingle(Arg.Any<Expression<Func<Delegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(existing);
        applicationsRepository.GetSingle(Arg.Any<Expression<Func<Applications, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Applications)null!);

        // Act
        Func<Task> act = async () => await service.Update(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        await repository.DidNotReceive().Update(Arg.Any<Delegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_CallsRepository()
    {
        // Arrange
        var id = Guid.NewGuid();
        var operatorId = Guid.NewGuid();
        repository.Delete(Arg.Any<Expression<Func<Delegations, bool>>>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await service.Delete(id, operatorId, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeTrue();
        await repository.Received(1).Delete(Arg.Any<Expression<Func<Delegations, bool>>>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
