// <copyright file="CphDelegationsServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Roles;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Requests.Delegations.Commands.Create;
using Defra.Identity.Requests.Delegations.Commands.Delete;
using Defra.Identity.Requests.Delegations.Commands.Update;
using Defra.Identity.Requests.Delegations.Queries;
using Defra.Identity.Services.Common.Builders.Strategy.Factories;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Delegations;
using Defra.Identity.Services.Tests.Delegations.TestData;
using Defra.Identity.Test.Utilities.Repository;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

public class CphDelegationsServiceTests
{
    private readonly ICphDelegationsRepository repository = Substitute.For<ICphDelegationsRepository>();
    private readonly IUsersRepository userRepository = Substitute.For<IUsersRepository>();
    private readonly ICphRepository cphRepository = Substitute.For<ICphRepository>();
    private readonly IRoleRepository roleRepository = Substitute.For<IRoleRepository>();
    private readonly IOperatorContext operatorContext = Substitute.For<IOperatorContext>();

    private readonly IStrategyBuilderFactory<CphDelegationsService> strategyBuilderFactory = new StrategyBuilderFactory<CphDelegationsService>();

    private readonly IValidator<CreateCphDelegation> createCphDelegationValidator = new CreateCphDelegationValidator();
    private readonly IValidator<UpdateCphDelegationById> updateCphDelegationValidator = new UpdateCphDelegationValidator();

    private readonly ILogger<CphDelegationsService> logger = Substitute.For<ILogger<CphDelegationsService>>();

    private readonly CphDelegationsService service;

    private readonly Guid mockOperatorId = new Guid("d7c98e62-af07-46ae-8b93-4765bfdb81c5");

    public CphDelegationsServiceTests()
    {
        service = new CphDelegationsService(
            repository,
            userRepository,
            cphRepository,
            roleRepository,
            operatorContext,
            strategyBuilderFactory,
            createCphDelegationValidator,
            updateCphDelegationValidator,
            logger);

        operatorContext.OperatorId.Returns(mockOperatorId);
    }

    [Fact]
    public async Task GetAll_ReturnsDelegations()
    {
        // Arrange
        var request = new GetCphDelegations();

        repository.GetList(Arg.Any<Expression<Func<CountyParishHoldingDelegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(CphDelegationsRepositoryMockingHelper.GetDelegationEntities().ToList());

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
        var request = new GetCphDelegationById
        {
            Id = new Guid("219de60c-9d5e-4f5c-a88c-c435cd8423b4"),
        };

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldingDelegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphDelegationsRepositoryMockingHelper.GetDelegationEntities()));

        // Act
        var result = await service.Get(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(new Guid("219de60c-9d5e-4f5c-a88c-c435cd8423b4"));
    }

    [Fact]
    public async Task Get_DelegationDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var request = new GetCphDelegationById
        {
            Id = Guid.NewGuid(),
        };

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldingDelegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((CountyParishHoldingDelegations)null!);

        // Act
        Func<Task> act = async () => await service.Get(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Create_CreatesAndReturnsDelegation_WhenReferencesExist()
    {
        // Arrange
        var mockCphId = new Guid("c630a7d8-4a75-4324-84de-b2b098617f71");
        var mockDelegatedUserRoleId = new Guid("b5cac49e-e5e4-47ee-bd6e-d8bc09694872");
        var mockInvitationExpiresAt = DateTime.Now.AddDays(2).ToUniversalTime();
        const string mockDelegatedUserEmail = "test200@test.com";

        var mockCphEntity = new CountyParishHoldings()
        {
            Id = mockCphId, Identifier = "22/001/0001",
        };

        var mockDelegatingUserEntity = new UserAccounts()
        {
            Id = new Guid("bbf9c0bf-461c-4186-8edd-bc51fdf2f053"), DisplayName = "Test User 100", EmailAddress = "test100@test.com",
        };

        var mockDelegatedUserEntity = new UserAccounts()
        {
            Id = new Guid("c5e29626-4a68-4125-a836-dec615e86386"), DisplayName = "Test User 200", EmailAddress = mockDelegatedUserEmail,
        };

        var mockDelegatedUserRoleEntity = new Roles()
        {
            Id = mockDelegatedUserRoleId, Name = "Test Role 100",
        };

        var request = new CreateCphDelegation
        {
            CountyParishHoldingId = mockCphId,
            DelegatingUserId = mockDelegatingUserEntity.Id,
            DelegatedUserId = mockDelegatedUserEntity.Id,
            DelegatedUserEmail = mockDelegatedUserEmail,
            DelegatedUserRoleId = mockDelegatedUserRoleId,
        };

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockCphEntity));

        cphRepository.ValidateReferenceById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        roleRepository.GetSingle(Arg.Any<Expression<Func<Roles, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatedUserRoleEntity));

        roleRepository.ValidateReferenceById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        userRepository.GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatingUserEntity, mockDelegatedUserEntity));

        userRepository.ValidateReferenceById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        var mockCreatedEntity = new CountyParishHoldingDelegations
        {
            Id = new Guid("399d00e3-5897-4e3c-bbcb-fe866ad3ef17"),
            CountyParishHoldingId = mockCphEntity.Id,
            CountyParishHolding = mockCphEntity,
            DelegatingUserId = request.DelegatingUserId,
            DelegatingUser = mockDelegatingUserEntity,
            DelegatedUserId = request.DelegatedUserId,
            DelegatedUser = mockDelegatedUserEntity,
            DelegatedUserRoleId = mockDelegatedUserRoleEntity.Id,
            DelegatedUserRole = mockDelegatedUserRoleEntity,
            DelegatedUserEmail = request.DelegatedUserEmail,
            InvitationToken = string.Empty,
            InvitationExpiresAt = mockInvitationExpiresAt,
            CreatedById = mockOperatorId,
        };

        repository.Create(Arg.Any<CountyParishHoldingDelegations>(), Arg.Any<CancellationToken>())
            .Returns(mockCreatedEntity);

        // Act
        var result = await service.Create(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(
            x =>
            {
                x.ShouldNotBeNull();
                x.Id.ShouldBe(new Guid("399d00e3-5897-4e3c-bbcb-fe866ad3ef17"));
                x.CountyParishHoldingId.ShouldBe(mockCphId);
                x.DelegatingUserId.ShouldBe(mockDelegatingUserEntity.Id);
                x.DelegatingUserName.ShouldBe(mockDelegatingUserEntity.DisplayName);
                x.DelegatedUserId.ShouldBe(mockDelegatedUserEntity.Id);
                x.DelegatedUserName.ShouldBe(mockDelegatedUserEntity.DisplayName);
                x.DelegatedUserRoleId.ShouldBe(mockDelegatedUserRoleEntity.Id);
                x.DelegatedUserRoleName.ShouldBe(mockDelegatedUserRoleEntity.Name);
                x.DelegatedUserEmail.ShouldBe(mockDelegatedUserEmail);
                x.InvitationExpiresAt.ShouldBe(mockInvitationExpiresAt);
                x.InvitationAcceptedAt.ShouldBe(null);
                x.InvitationRejectedAt.ShouldBe(null);
                x.RevokedAt.ShouldBe(null);
                x.RevokedById.ShouldBe(null);
                x.RevokedByName.ShouldBe(null);
                x.ExpiresAt.ShouldBe(null);
            });

        await cphRepository.Received(1).ValidateReferenceById(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await roleRepository.Received(1).ValidateReferenceById(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await userRepository.Received(2).ValidateReferenceById(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await repository.Received(1).Create(Arg.Any<CountyParishHoldingDelegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_ThrowsNotFoundException_WhenCphDoesNotExist()
    {
        // Arrange
        var mockDelegatingUserEntity = new UserAccounts()
        {
            Id = new Guid("bbf9c0bf-461c-4186-8edd-bc51fdf2f053"), DisplayName = "Test User 100", EmailAddress = "test100@test.com",
        };

        var mockDelegatedUserEntity = new UserAccounts()
        {
            Id = new Guid("c5e29626-4a68-4125-a836-dec615e86386"), DisplayName = "Test User 200", EmailAddress = "test200@test.com",
        };

        var mockDelegatedUserRoleEntity = new Roles()
        {
            Id = new Guid("b5cac49e-e5e4-47ee-bd6e-d8bc09694872"), Name = "Test Role 100",
        };

        var request = new CreateCphDelegation
        {
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatingUserId = mockDelegatingUserEntity.Id,
            DelegatedUserId = mockDelegatedUserEntity.Id,
            DelegatedUserEmail = mockDelegatedUserEntity.EmailAddress,
            DelegatedUserRoleId = mockDelegatedUserRoleEntity.Id,
        };

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((CountyParishHoldings)null!);

        roleRepository.GetSingle(Arg.Any<Expression<Func<Roles, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatedUserRoleEntity));

        userRepository.GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatingUserEntity, mockDelegatedUserEntity));

        // Act
        Func<Task> act = async () => await service.Create(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        await repository.DidNotReceive().Create(Arg.Any<CountyParishHoldingDelegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_ThrowsNotFoundException_WhenDelegatingUserDoesNotExist()
    {
        // Arrange
        var mockCphEntity = new CountyParishHoldings()
        {
            Id = new Guid("c630a7d8-4a75-4324-84de-b2b098617f71"), Identifier = "22/001/0001",
        };

        var mockDelegatedUserEntity = new UserAccounts()
        {
            Id = new Guid("c5e29626-4a68-4125-a836-dec615e86386"), DisplayName = "Test User 200", EmailAddress = "test200@test.com",
        };

        var mockDelegatedUserRoleEntity = new Roles()
        {
            Id = new Guid("b5cac49e-e5e4-47ee-bd6e-d8bc09694872"), Name = "Test Role 100",
        };

        var request = new CreateCphDelegation
        {
            CountyParishHoldingId = mockCphEntity.Id,
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserId = mockDelegatedUserEntity.Id,
            DelegatedUserEmail = mockDelegatedUserEntity.EmailAddress,
            DelegatedUserRoleId = mockDelegatedUserRoleEntity.Id,
        };

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockCphEntity));

        roleRepository.GetSingle(Arg.Any<Expression<Func<Roles, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatedUserRoleEntity));

        userRepository.GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatedUserEntity));

        // Act
        Func<Task> act = async () => await service.Create(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        await repository.DidNotReceive().Create(Arg.Any<CountyParishHoldingDelegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_ThrowsNotFoundException_WhenDelegatedUserDoesNotExist()
    {
        // Arrange
        var mockCphEntity = new CountyParishHoldings()
        {
            Id = new Guid("c630a7d8-4a75-4324-84de-b2b098617f71"), Identifier = "22/001/0001",
        };

        var mockDelegatingUserEntity = new UserAccounts()
        {
            Id = new Guid("bbf9c0bf-461c-4186-8edd-bc51fdf2f053"), DisplayName = "Test User 100", EmailAddress = "test100@test.com",
        };

        var mockDelegatedUserRoleEntity = new Roles()
        {
            Id = new Guid("b5cac49e-e5e4-47ee-bd6e-d8bc09694872"), Name = "Test Role 100",
        };

        var request = new CreateCphDelegation
        {
            CountyParishHoldingId = mockCphEntity.Id,
            DelegatingUserId = mockDelegatingUserEntity.Id,
            DelegatedUserId = Guid.NewGuid(),
            DelegatedUserEmail = "test200@test.com",
            DelegatedUserRoleId = mockDelegatedUserRoleEntity.Id,
        };

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockCphEntity));

        roleRepository.GetSingle(Arg.Any<Expression<Func<Roles, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatedUserRoleEntity));

        userRepository.GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatingUserEntity));

        // Act
        Func<Task> act = async () => await service.Create(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        await repository.DidNotReceive().Create(Arg.Any<CountyParishHoldingDelegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_ThrowsNotFoundException_WhenDelegatedUserRoleDoesNotExist()
    {
        // Arrange
        var mockCphEntity = new CountyParishHoldings()
        {
            Id = new Guid("c630a7d8-4a75-4324-84de-b2b098617f71"), Identifier = "22/001/0001",
        };

        var mockDelegatingUserEntity = new UserAccounts()
        {
            Id = new Guid("bbf9c0bf-461c-4186-8edd-bc51fdf2f053"), DisplayName = "Test User 100", EmailAddress = "test100@test.com",
        };

        var mockDelegatedUserEntity = new UserAccounts()
        {
            Id = new Guid("c5e29626-4a68-4125-a836-dec615e86386"), DisplayName = "Test User 200", EmailAddress = "test200@test.com",
        };

        var request = new CreateCphDelegation
        {
            CountyParishHoldingId = mockCphEntity.Id,
            DelegatingUserId = mockDelegatingUserEntity.Id,
            DelegatedUserId = mockDelegatedUserEntity.Id,
            DelegatedUserEmail = mockDelegatedUserEntity.EmailAddress,
            DelegatedUserRoleId = Guid.NewGuid(),
        };

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockCphEntity));

        roleRepository.GetSingle(Arg.Any<Expression<Func<Roles, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, (Roles[])[]));

        userRepository.GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatingUserEntity, mockDelegatedUserEntity));

        // Act
        Func<Task> act = async () => await service.Create(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        await repository.DidNotReceive().Create(Arg.Any<CountyParishHoldingDelegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_CallsRepository()
    {
        // Arrange
        var request = new DeleteCphDelegationById()
        {
            Id = Guid.NewGuid(),
        };

        repository.Delete(Arg.Any<Expression<Func<CountyParishHoldingDelegations, bool>>>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await service.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeTrue();
        await repository.Received(1).Delete(Arg.Any<Expression<Func<CountyParishHoldingDelegations, bool>>>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_UpdatesAndReturnsDelegation_WhenReferencesExist()
    {
        // Arrange
        var mockId = new Guid("044e3f8a-5980-47df-8502-4c0cd0cdf887");
        var mockCphId = new Guid("c630a7d8-4a75-4324-84de-b2b098617f71");
        var mockDelegatedUserRoleId = new Guid("b5cac49e-e5e4-47ee-bd6e-d8bc09694872");
        var mockInvitationExpiresAt = DateTime.Now.AddDays(2).ToUniversalTime();
        const string mockDelegatedUserEmail = "test200@test.com";

        var mockCphEntity = new CountyParishHoldings()
        {
            Id = mockCphId, Identifier = "22/001/0001",
        };

        var mockDelegatingUserEntity = new UserAccounts()
        {
            Id = new Guid("bbf9c0bf-461c-4186-8edd-bc51fdf2f053"), DisplayName = "Test User 100", EmailAddress = "test100@test.com",
        };

        var mockDelegatedUserEntity = new UserAccounts()
        {
            Id = new Guid("c5e29626-4a68-4125-a836-dec615e86386"), DisplayName = "Test User 200", EmailAddress = mockDelegatedUserEmail,
        };

        var mockDelegatedUserRoleEntity = new Roles()
        {
            Id = mockDelegatedUserRoleId, Name = "Test Role 100",
        };

        var request = new UpdateCphDelegationById()
        {
            Id = mockId,
            CountyParishHoldingId = mockCphId,
            DelegatingUserId = mockDelegatingUserEntity.Id,
            DelegatedUserId = mockDelegatedUserEntity.Id,
            DelegatedUserEmail = mockDelegatedUserEmail,
            DelegatedUserRoleId = mockDelegatedUserRoleId,
        };

        var mockCphDelegationEntity = new CountyParishHoldingDelegations
        {
            Id = mockId,
            CountyParishHoldingId = mockCphEntity.Id,
            CountyParishHolding = mockCphEntity,
            DelegatingUserId = request.DelegatingUserId,
            DelegatingUser = mockDelegatingUserEntity,
            DelegatedUserId = request.DelegatedUserId,
            DelegatedUser = mockDelegatedUserEntity,
            DelegatedUserRoleId = mockDelegatedUserRoleEntity.Id,
            DelegatedUserRole = mockDelegatedUserRoleEntity,
            DelegatedUserEmail = request.DelegatedUserEmail,
            InvitationToken = string.Empty,
            InvitationExpiresAt = mockInvitationExpiresAt,
            CreatedById = mockOperatorId,
        };

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldingDelegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockCphDelegationEntity));

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockCphEntity));

        cphRepository.ValidateReferenceById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        roleRepository.GetSingle(Arg.Any<Expression<Func<Roles, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatedUserRoleEntity));

        roleRepository.ValidateReferenceById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        userRepository.GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatingUserEntity, mockDelegatedUserEntity));

        userRepository.ValidateReferenceById(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        repository.Update(Arg.Any<CountyParishHoldingDelegations>(), Arg.Any<CancellationToken>())
            .Returns(mockCphDelegationEntity);

        // Act
        var result = await service.Update(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(
            x =>
            {
                x.ShouldNotBeNull();
                x.Id.ShouldBe(mockId);
                x.CountyParishHoldingId.ShouldBe(mockCphId);
                x.DelegatingUserId.ShouldBe(mockDelegatingUserEntity.Id);
                x.DelegatingUserName.ShouldBe(mockDelegatingUserEntity.DisplayName);
                x.DelegatedUserId.ShouldBe(mockDelegatedUserEntity.Id);
                x.DelegatedUserName.ShouldBe(mockDelegatedUserEntity.DisplayName);
                x.DelegatedUserRoleId.ShouldBe(mockDelegatedUserRoleEntity.Id);
                x.DelegatedUserRoleName.ShouldBe(mockDelegatedUserRoleEntity.Name);
                x.DelegatedUserEmail.ShouldBe(mockDelegatedUserEmail);
                x.InvitationExpiresAt.ShouldBe(mockInvitationExpiresAt);
                x.InvitationAcceptedAt.ShouldBe(null);
                x.InvitationRejectedAt.ShouldBe(null);
                x.RevokedAt.ShouldBe(null);
                x.RevokedById.ShouldBe(null);
                x.RevokedByName.ShouldBe(null);
                x.ExpiresAt.ShouldBe(null);
            });

        await repository.Received(1).GetSingle(Arg.Any<Expression<Func<CountyParishHoldingDelegations, bool>>>(), Arg.Any<CancellationToken>());
        await cphRepository.Received(1).ValidateReferenceById(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await roleRepository.Received(1).ValidateReferenceById(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await userRepository.Received(2).ValidateReferenceById(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await repository.Received(1).Update(Arg.Any<CountyParishHoldingDelegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_ThrowsNotFoundException_WhenDelegationDoesNotExist()
    {
        // Arrange
        var request = new UpdateCphDelegationById()
        {
            Id = Guid.NewGuid(),
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserId = Guid.NewGuid(),
            DelegatedUserEmail = "test200@test.com",
            DelegatedUserRoleId = Guid.NewGuid(),
        };

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldingDelegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, (CountyParishHoldingDelegations[])[]));

        // Act
        Func<Task> act = async () => await service.Update(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        await cphRepository.DidNotReceive().GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>());
        await roleRepository.DidNotReceive().GetSingle(Arg.Any<Expression<Func<Roles, bool>>>(), Arg.Any<CancellationToken>());
        await userRepository.DidNotReceive().GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>());
        await repository.DidNotReceive().Update(Arg.Any<CountyParishHoldingDelegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_ThrowsNotFoundException_WhenCphDoesNotExist()
    {
        // Arrange
        var mockId = new Guid("044e3f8a-5980-47df-8502-4c0cd0cdf887");

        var mockDelegatingUserEntity = new UserAccounts()
        {
            Id = new Guid("bbf9c0bf-461c-4186-8edd-bc51fdf2f053"), DisplayName = "Test User 100", EmailAddress = "test100@test.com",
        };

        var mockDelegatedUserEntity = new UserAccounts()
        {
            Id = new Guid("c5e29626-4a68-4125-a836-dec615e86386"), DisplayName = "Test User 200", EmailAddress = "test200@test.com",
        };

        var mockDelegatedUserRoleEntity = new Roles()
        {
            Id = new Guid("b5cac49e-e5e4-47ee-bd6e-d8bc09694872"), Name = "Test Role 100",
        };

        var request = new UpdateCphDelegationById()
        {
            Id = mockId,
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatingUserId = mockDelegatingUserEntity.Id,
            DelegatedUserId = mockDelegatedUserEntity.Id,
            DelegatedUserEmail = mockDelegatedUserEntity.EmailAddress,
            DelegatedUserRoleId = mockDelegatedUserRoleEntity.Id,
        };

        var mockExistingCphDelegationEntity = new CountyParishHoldingDelegations
        {
            Id = mockId,
            CountyParishHoldingId = Guid.NewGuid(),
            CountyParishHolding = new CountyParishHoldings(),
            DelegatingUserId = request.DelegatingUserId,
            DelegatingUser = mockDelegatingUserEntity,
            DelegatedUserId = request.DelegatedUserId,
            DelegatedUser = mockDelegatedUserEntity,
            DelegatedUserRoleId = mockDelegatedUserRoleEntity.Id,
            DelegatedUserRole = mockDelegatedUserRoleEntity,
            DelegatedUserEmail = request.DelegatedUserEmail,
            InvitationToken = string.Empty,
            InvitationExpiresAt = DateTime.Now.AddDays(2).ToUniversalTime(),
            CreatedById = Guid.NewGuid(),
        };

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldingDelegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockExistingCphDelegationEntity));

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((CountyParishHoldings)null!);

        roleRepository.GetSingle(Arg.Any<Expression<Func<Roles, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatedUserRoleEntity));

        userRepository.GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatingUserEntity, mockDelegatedUserEntity));

        // Act
        Func<Task> act = async () => await service.Update(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        await repository.DidNotReceive().Update(Arg.Any<CountyParishHoldingDelegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_ThrowsNotFoundException_WhenDelegatingUserDoesNotExist()
    {
        // Arrange
        var mockId = new Guid("044e3f8a-5980-47df-8502-4c0cd0cdf887");

        var mockCphEntity = new CountyParishHoldings()
        {
            Id = new Guid("c630a7d8-4a75-4324-84de-b2b098617f71"), Identifier = "22/001/0001",
        };

        var mockDelegatedUserEntity = new UserAccounts()
        {
            Id = new Guid("c5e29626-4a68-4125-a836-dec615e86386"), DisplayName = "Test User 200", EmailAddress = "test200@test.com",
        };

        var mockDelegatedUserRoleEntity = new Roles()
        {
            Id = new Guid("b5cac49e-e5e4-47ee-bd6e-d8bc09694872"), Name = "Test Role 100",
        };

        var request = new CreateCphDelegation
        {
            CountyParishHoldingId = mockCphEntity.Id,
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserId = mockDelegatedUserEntity.Id,
            DelegatedUserEmail = mockDelegatedUserEntity.EmailAddress,
            DelegatedUserRoleId = mockDelegatedUserRoleEntity.Id,
        };

        var mockExistingCphDelegationEntity = new CountyParishHoldingDelegations
        {
            Id = mockId,
            CountyParishHoldingId = mockCphEntity.Id,
            CountyParishHolding = mockCphEntity,
            DelegatingUserId = Guid.NewGuid(),
            DelegatingUser = new UserAccounts(),
            DelegatedUserId = request.DelegatedUserId,
            DelegatedUser = mockDelegatedUserEntity,
            DelegatedUserRoleId = mockDelegatedUserRoleEntity.Id,
            DelegatedUserRole = mockDelegatedUserRoleEntity,
            DelegatedUserEmail = request.DelegatedUserEmail,
            InvitationToken = string.Empty,
            InvitationExpiresAt = DateTime.Now.AddDays(2).ToUniversalTime(),
            CreatedById = Guid.NewGuid(),
        };

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldingDelegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockExistingCphDelegationEntity));

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockCphEntity));

        roleRepository.GetSingle(Arg.Any<Expression<Func<Roles, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatedUserRoleEntity));

        userRepository.GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatedUserEntity));

        // Act
        Func<Task> act = async () => await service.Create(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        await repository.DidNotReceive().Update(Arg.Any<CountyParishHoldingDelegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_ThrowsNotFoundException_WhenDelegatedUserDoesNotExist()
    {
        // Arrange
        var mockId = new Guid("044e3f8a-5980-47df-8502-4c0cd0cdf887");

        var mockCphEntity = new CountyParishHoldings()
        {
            Id = new Guid("c630a7d8-4a75-4324-84de-b2b098617f71"), Identifier = "22/001/0001",
        };

        var mockDelegatingUserEntity = new UserAccounts()
        {
            Id = new Guid("bbf9c0bf-461c-4186-8edd-bc51fdf2f053"), DisplayName = "Test User 100", EmailAddress = "test100@test.com",
        };

        var mockDelegatedUserRoleEntity = new Roles()
        {
            Id = new Guid("b5cac49e-e5e4-47ee-bd6e-d8bc09694872"), Name = "Test Role 100",
        };

        var request = new CreateCphDelegation
        {
            CountyParishHoldingId = mockCphEntity.Id,
            DelegatingUserId = mockDelegatingUserEntity.Id,
            DelegatedUserId = Guid.NewGuid(),
            DelegatedUserEmail = "test200@test.com",
            DelegatedUserRoleId = mockDelegatedUserRoleEntity.Id,
        };

        var mockExistingCphDelegationEntity = new CountyParishHoldingDelegations
        {
            Id = mockId,
            CountyParishHoldingId = mockCphEntity.Id,
            CountyParishHolding = mockCphEntity,
            DelegatingUserId = request.DelegatingUserId,
            DelegatingUser = mockDelegatingUserEntity,
            DelegatedUserId = Guid.NewGuid(),
            DelegatedUser = new UserAccounts(),
            DelegatedUserRoleId = mockDelegatedUserRoleEntity.Id,
            DelegatedUserRole = mockDelegatedUserRoleEntity,
            DelegatedUserEmail = request.DelegatedUserEmail,
            InvitationToken = string.Empty,
            InvitationExpiresAt = DateTime.Now.AddDays(2).ToUniversalTime(),
            CreatedById = Guid.NewGuid(),
        };

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldingDelegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockExistingCphDelegationEntity));

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockCphEntity));

        roleRepository.GetSingle(Arg.Any<Expression<Func<Roles, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatedUserRoleEntity));

        userRepository.GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatingUserEntity));

        // Act
        Func<Task> act = async () => await service.Create(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        await repository.DidNotReceive().Update(Arg.Any<CountyParishHoldingDelegations>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_ThrowsNotFoundException_WhenDelegatedUserRoleDoesNotExist()
    {
        // Arrange
        var mockId = new Guid("044e3f8a-5980-47df-8502-4c0cd0cdf887");

        var mockCphEntity = new CountyParishHoldings()
        {
            Id = new Guid("c630a7d8-4a75-4324-84de-b2b098617f71"), Identifier = "22/001/0001",
        };

        var mockDelegatingUserEntity = new UserAccounts()
        {
            Id = new Guid("bbf9c0bf-461c-4186-8edd-bc51fdf2f053"), DisplayName = "Test User 100", EmailAddress = "test100@test.com",
        };

        var mockDelegatedUserEntity = new UserAccounts()
        {
            Id = new Guid("c5e29626-4a68-4125-a836-dec615e86386"), DisplayName = "Test User 200", EmailAddress = "test200@test.com",
        };

        var request = new CreateCphDelegation
        {
            CountyParishHoldingId = mockCphEntity.Id,
            DelegatingUserId = mockDelegatingUserEntity.Id,
            DelegatedUserId = mockDelegatedUserEntity.Id,
            DelegatedUserEmail = mockDelegatedUserEntity.EmailAddress,
            DelegatedUserRoleId = Guid.NewGuid(),
        };

        var mockExistingCphDelegationEntity = new CountyParishHoldingDelegations
        {
            Id = mockId,
            CountyParishHoldingId = mockCphEntity.Id,
            CountyParishHolding = mockCphEntity,
            DelegatingUserId = request.DelegatingUserId,
            DelegatingUser = mockDelegatingUserEntity,
            DelegatedUserId = Guid.NewGuid(),
            DelegatedUser = new UserAccounts(),
            DelegatedUserRoleId = Guid.NewGuid(),
            DelegatedUserRole = new Roles(),
            DelegatedUserEmail = request.DelegatedUserEmail,
            InvitationToken = string.Empty,
            InvitationExpiresAt = DateTime.Now.AddDays(2).ToUniversalTime(),
            CreatedById = Guid.NewGuid(),
        };

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldingDelegations, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockExistingCphDelegationEntity));

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockCphEntity));

        roleRepository.GetSingle(Arg.Any<Expression<Func<Roles, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, (Roles[])[]));

        userRepository.GetSingle(Arg.Any<Expression<Func<UserAccounts, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, mockDelegatingUserEntity, mockDelegatedUserEntity));

        // Act
        Func<Task> act = async () => await service.Create(request, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        await repository.DidNotReceive().Update(Arg.Any<CountyParishHoldingDelegations>(), Arg.Any<CancellationToken>());
    }
}
