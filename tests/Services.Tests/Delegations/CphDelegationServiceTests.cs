// <copyright file="CphDelegationServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Delegations;

using System.ComponentModel;
using Defra.Identity.Messaging;
using Defra.Identity.Messaging.Services;
using Defra.Identity.Models.Requests.Delegations.Commands;
using Defra.Identity.Models.Requests.Delegations.Queries;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Roles;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Common.Exceptions;
using Defra.Identity.Services.Common.Strategy.Factories;
using Defra.Identity.Services.Delegations;
using Defra.Identity.Services.Delegations.Injection;
using Defra.Identity.Test.Utilities.Assertions;
using Defra.Identity.Test.Utilities.Comparison;
using Defra.Identity.Test.Utilities.Repository;
using Defra.Identity.Test.Utilities.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;

#pragma warning disable SA1201 // Elements should appear in the correct order // Ordering is important for test readability
#pragma warning disable xUnit1047 // Entities not serializable for test explorer // Accepted

public class CphDelegationServiceTests
{
    private readonly ICphDelegationsRepository cphDelegationsRepository = Substitute.For<ICphDelegationsRepository>();
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly ICphRepository cphRepository = Substitute.For<ICphRepository>();
    private readonly IRoleRepository roleRepository = Substitute.For<IRoleRepository>();

    private readonly IMessagingFactory messagingFactory = Substitute.For<IMessagingFactory>();

    private readonly IStrategyBuilderFactory<CphDelegationService> strategyBuilderFactory =
        new StrategyBuilderFactory<CphDelegationService>();

    private readonly IValidator<CreateCphDelegation> createCphDelegationValidator =
        Substitute.For<IValidator<CreateCphDelegation>>();

    private readonly ILogger<CphDelegationService> logger =
        DefraLoggerExtensions.CreateNSubstituteLogger<CphDelegationService>();

    private readonly IOperatorContext? operatorContext = Substitute.For<IOperatorContext>();

    private readonly SutProvider<CphDelegationService> sut;

    public CphDelegationServiceTests()
    {
        var repoContext =
            new CphDelegationSvcRepoContext(cphDelegationsRepository, userRepository, cphRepository, roleRepository);

        sut = SutProvider<CphDelegationService>.CreateFor(
            context => new CphDelegationService(
                repoContext,
                context!,
                messagingFactory,
                strategyBuilderFactory,
                createCphDelegationValidator,
                logger),
            operatorContext);
    }

    [Fact]
    [Description(
        "GetAll returns all delegations including excluding deleted, expired, invalid refs and no valid cph owner assignment")]
    public async Task
        GetAll_Returns_All_Delegations_Excluding_Deleted_And_Expired_And_Invalid_Refs_And_No_Valid_Cph_Owner_Assignment()
    {
        // Arrange
        MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository)
            .WithData(
            [
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph1ValidPendingWithinInviteExpiry,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph2ReferencedCphExpiredPending,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph3ReferencedCphDeletedPending,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph4ValidAcceptedWithinInviteExpiry,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph7CphAssignmentDeletedPending,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph9ValidRejected,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph10ValidAcceptedWithinInviteExpiry,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidAcceptedInviteNowExpired,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingInviteExpired,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationExpired,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidAcceptedDelegationExpired,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationRevoked,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidAcceptedDelegationRevoked,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationDeleted,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidAcceptedDelegationDeleted,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph12NoCphAssignmentsPending,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph16CphAssignedToDeletedUser3Pending,
                TestData.CphDelegations.FromUser1NotDeleted.ToUser4NotDeleted.ToCph1ValidPendingWithinInviteExpiry,
                TestData.CphDelegations.FromUser2NotDeleted.ToUser1NotDeleted.ToCph13ValidPendingWithinInviteExpiry,
                TestData.CphDelegations.FromUser2NotDeleted.ToUser1NotDeleted.ToCph14ValidAcceptedWithinInviteExpiry,
                TestData.CphDelegations.FromUser2NotDeleted.ToUser3Deleted.ToCph14ValidPendingWithinInviteExpiry,
                TestData.CphDelegations.FromUser3Deleted.ToUser2NotDeleted.ToCph15ValidPendingWithinInviteExpiry
            ]);

        // Act
        var result = await sut.WithoutOperatorId.GetAll(TestContext.Current.CancellationToken);

        // Assert
        result.Count.ShouldBe(11);

        result[0].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted
            .ToUser2NotDeleted.ToCph1ValidPendingWithinInviteExpiry));

        result[1].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted
            .ToUser2NotDeleted.ToCph4ValidAcceptedWithinInviteExpiry));

        result[2].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted
            .ToUser2NotDeleted.ToCph9ValidRejected));

        result[3].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted
            .ToUser2NotDeleted.ToCph10ValidAcceptedWithinInviteExpiry));

        result[4].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted
            .ToUser2NotDeleted.ToCph11ValidAcceptedInviteNowExpired));

        result[5].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted
            .ToUser2NotDeleted.ToCph11ValidPendingInviteExpired));

        result[6].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted
            .ToUser2NotDeleted.ToCph11ValidPendingDelegationRevoked));

        result[7].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted
            .ToUser2NotDeleted.ToCph11ValidAcceptedDelegationRevoked));

        result[8].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted
            .ToUser4NotDeleted.ToCph1ValidPendingWithinInviteExpiry));

        result[9].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser2NotDeleted
            .ToUser1NotDeleted.ToCph13ValidPendingWithinInviteExpiry));

        result[10].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser2NotDeleted
            .ToUser1NotDeleted.ToCph14ValidAcceptedWithinInviteExpiry));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Executing get all county parish holding delegations [county parish holding delegation]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Successfully executed get all county parish holding delegations [county parish holding delegation]");
    }

    public static IEnumerable<TheoryDataRow<CountyParishHoldingDelegations>>
        GetTestDataNotDeletedOrExpiredAndValidRefsAndHasValidCphOwnerAssignment =>
        new List<TheoryDataRow<CountyParishHoldingDelegations>>
        {
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph1ValidPendingWithinInviteExpiry,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph4ValidAcceptedWithinInviteExpiry,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph9ValidRejected,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph10ValidAcceptedWithinInviteExpiry,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidAcceptedInviteNowExpired,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingInviteExpired,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationRevoked,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidAcceptedDelegationRevoked,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser4NotDeleted.ToCph1ValidPendingWithinInviteExpiry,
            TestData.CphDelegations.FromUser2NotDeleted.ToUser1NotDeleted.ToCph13ValidPendingWithinInviteExpiry,
            TestData.CphDelegations.FromUser2NotDeleted.ToUser1NotDeleted.ToCph14ValidAcceptedWithinInviteExpiry,
        };

    [Theory]
    [MemberData(nameof(GetTestDataNotDeletedOrExpiredAndValidRefsAndHasValidCphOwnerAssignment))]
    [Description(
        "Get returns the requested delegation if not expired or deleted and has valid refs and valid cph owner assignment")]
    public async Task
        Get_Returns_Requested_Delegation_If_Not_Expired_Or_Deleted_And_Has_Valid_Refs_And_Valid_Cph_Owner_Assignment(
            CountyParishHoldingDelegations delegationEntity)
    {
        // Arrange
        var delegationEntityForEvaluation = TestData.GetEntitiesOfType<CountyParishHoldingDelegations>()
            .First(delegation => delegation.Id == delegationEntity.Id);

        var request = new GetCphDelegationById { Id = delegationEntity.Id };

        MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository)
            .WithData(
            [
                delegationEntity,
            ]);

        // Act
        var result = await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(delegationEntityForEvaluation));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get county parish holding delegation [county parish holding delegation] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get county parish holding delegation [county parish holding delegation] with id {request.Id}");
    }

    public static IEnumerable<TheoryDataRow<CountyParishHoldingDelegations>>
        GetTestDataDeletedOrExpiredOrInvalidValidRefsIrInvalidOrNoCphOwnerAssignment =>
        new List<TheoryDataRow<CountyParishHoldingDelegations>>
        {
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph2ReferencedCphExpiredPending,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph3ReferencedCphDeletedPending,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph7CphAssignmentDeletedPending,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationExpired,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidAcceptedDelegationExpired,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationDeleted,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidAcceptedDelegationDeleted,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph12NoCphAssignmentsPending,
            TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph16CphAssignedToDeletedUser3Pending,
            TestData.CphDelegations.FromUser2NotDeleted.ToUser3Deleted.ToCph14ValidPendingWithinInviteExpiry,
            TestData.CphDelegations.FromUser3Deleted.ToUser2NotDeleted.ToCph15ValidPendingWithinInviteExpiry,
        };

    [Theory]
    [MemberData(nameof(GetTestDataDeletedOrExpiredOrInvalidValidRefsIrInvalidOrNoCphOwnerAssignment))]
    [Description(
        "Get throws not found exception when requested delegation is expired or deleted or has invalid refs or no valid cph owner assignment")]
    public async Task
        Get_Throws_NotFound_Exception_When_Requested_Delegation_Expired_Or_Deleted_Or_Invalid_Refs_Or_No_Valid_Cph_Owner_Assignment(
            CountyParishHoldingDelegations delegationEntity)
    {
        // Arrange
        var request = new GetCphDelegationById() { Id = delegationEntity.Id };

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository)
            .WithData(
            [
                delegationEntity,
            ]);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding delegation not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get county parish holding delegation [county parish holding delegation] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"County parish holding delegation with id {request.Id} not found");
    }

    [Fact]
    [Description("Get throws not found exception when requested delegation does not exist")]
    public async Task Get_Throws_NotFound_Exception_When_Requested_Delegation_Does_Not_Exist()
    {
        // Arrange
        var request = new GetCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding delegation not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get county parish holding delegation [county parish holding delegation] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"County parish holding delegation with id {request.Id} not found");
    }

    [Fact]
    [Description("Create send email message and returns new delegation")]
    public async Task Create_Sends_Email_Message_And_Returns_New_Delegation()
    {
        // Arrange
        var delegatingUser1NotDeleted = TestData.User.User1NotDeleted;
        var delegatedUser2NotDeleted = TestData.User.User2NotDeleted;
        var cph1WithAllowedSpeciesNotExpiredOrDeleted = TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted;
        var delegateRole1 = TestData.Role.Role1;

        var idForNewEntity = Guid.NewGuid();

        var request =
            new CreateCphDelegation()
            {
                CountyParishHoldingId = cph1WithAllowedSpeciesNotExpiredOrDeleted.Id,
                DelegatingUserId = delegatingUser1NotDeleted.Id,
                DelegatedUserEmail = delegatedUser2NotDeleted.EmailAddress.ToUpperInvariant(),
                DelegatedUserRoleId = delegateRole1.Id,
            };

        var validatorContext = MockValidatorContext<CreateCphDelegation>.CreateFor(createCphDelegationValidator);

        MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData(
            [
                delegatingUser1NotDeleted,
                delegatedUser2NotDeleted
            ]);

        MockRepositoryContext<Roles>.CreateFor(roleRepository)
            .WithData([delegateRole1]);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(cphRepository)
            .WithData([cph1WithAllowedSpeciesNotExpiredOrDeleted]);

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository)
            .WithNextCreateEntityId(idForNewEntity)
            .WithCreateResult(entity =>
            {
                entity.CountyParishHolding = cph1WithAllowedSpeciesNotExpiredOrDeleted;
                entity.DelegatingUser = delegatingUser1NotDeleted;
                entity.DelegatedUser = delegatedUser2NotDeleted;
                entity.DelegatedUserRole = delegateRole1;

                entity.CountyParishHolding.ApplicationUserAccountHoldingAssignments.Add(
                    new UserAccountCountyParishHoldingAssignments()
                    {
                        Id = Guid.NewGuid(),
                        CountyParishHoldingId = cph1WithAllowedSpeciesNotExpiredOrDeleted.Id,
                        CountyParishHolding = cph1WithAllowedSpeciesNotExpiredOrDeleted,
                        UserAccountId = delegatingUser1NotDeleted.Id,
                        UserAccount = delegatingUser1NotDeleted,
                        RoleId = delegateRole1.Id,
                        CreatedById = delegatingUser1NotDeleted.Id,
                        CreatedByUser = delegatingUser1NotDeleted,
                    });

                return entity;
            });

        // Act
        var result = await sut.WithOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.CreateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastCreateResult.ShouldNotBeNull();

        repositoryContext.Calls.LastCreateResult.ShouldSatisfyAllConditions(createdEntity =>
        {
            createdEntity.Id.ShouldBe(idForNewEntity);
            createdEntity.CountyParishHoldingId.ShouldBe(request.CountyParishHoldingId);
            createdEntity.DelegatingUserId.ShouldBe(request.DelegatingUserId);
            createdEntity.DelegatedUserId.ShouldBe(delegatedUser2NotDeleted.Id);
            createdEntity.DelegatedUserEmail.ShouldBe(request.DelegatedUserEmail.ToLowerInvariant());
            createdEntity.DelegatedUserRoleId.ShouldBe(request.DelegatedUserRoleId);
            createdEntity.InvitationExpiresAt.ShouldBeCloseToUtcNowAddDays(2);
            createdEntity.InvitationAcceptedAt.ShouldBeNull();
            createdEntity.InvitationRejectedAt.ShouldBeNull();
            createdEntity.InvitationToken.ShouldBe(string.Empty);
            createdEntity.RevokedAt.ShouldBeNull();
            createdEntity.RevokedById.ShouldBeNull();
            createdEntity.ExpiresAt.ShouldBeNull();
            createdEntity.DeletedById.ShouldBeNull();
            createdEntity.DeletedAt.ShouldBeNull();
            createdEntity.CreatedById.ShouldBe(operatorContext!.OperatorId);
            createdEntity.CreatedAt.ShouldBeCloseToUtcNow();
        });

        messagingFactory.ShouldSatisfyAllConditions(
            Assertions.ShouldHaveQueuedDelegationEmailMessage(
                idForNewEntity,
                delegatedUser2NotDeleted.EmailAddress,
                MessageTemplateTypes.Delegation.DelegationInvitee));

        result.ShouldSatisfyAllConditions(
            Assertions.ShouldMapFromEntity(repositoryContext.Calls.LastCreateResult));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing create county parish holding delegation [county parish holding delegation] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing create county parish holding delegation [county parish holding delegation] by operator {operatorContext.OperatorId}");
    }

    [Fact]
    [Description("Create throws not found exception when referenced cph expired")]
    public async Task Create_Throws_NotFound_Exception_When_Referenced_Cph_Expired()
    {
        // Arrange
        var delegatingUser1NotDeleted = TestData.User.User1NotDeleted;
        var delegatedUser2NotDeleted = TestData.User.User2NotDeleted;
        var cph2ExpiredButNotDeleted = TestData.Cph.Cph2ExpiredButNotDeleted;
        var delegateRole1 = TestData.Role.Role1;

        var request =
            new CreateCphDelegation()
            {
                CountyParishHoldingId = cph2ExpiredButNotDeleted.Id,
                DelegatingUserId = delegatingUser1NotDeleted.Id,
                DelegatedUserEmail = delegatedUser2NotDeleted.EmailAddress.ToUpperInvariant(),
                DelegatedUserRoleId = delegateRole1.Id,
            };

        var validatorContext = MockValidatorContext<CreateCphDelegation>.CreateFor(createCphDelegationValidator);

        MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData(
            [
                delegatingUser1NotDeleted,
                delegatedUser2NotDeleted
            ]);

        MockRepositoryContext<Roles>.CreateFor(roleRepository)
            .WithData([delegateRole1]);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(cphRepository)
            .WithData([cph2ExpiredButNotDeleted]);

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding must exist and not be deleted or expired");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing create county parish holding delegation [county parish holding delegation] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute create county parish holding delegation [county parish holding delegation] failed reference rule 'County parish holding must exist and not be deleted or expired'");
    }

    [Fact]
    [Description("Create throws not found exception when referenced cph is deleted")]
    public async Task Create_Throws_NotFound_Exception_When_Referenced_Cph_Deleted()
    {
        // Arrange
        var delegatingUser1NotDeleted = TestData.User.User1NotDeleted;
        var delegatedUser2NotDeleted = TestData.User.User2NotDeleted;
        var cph3DeletedButNotExpired = TestData.Cph.Cph3DeletedButNotExpired;
        var delegateRole1 = TestData.Role.Role1;

        var request =
            new CreateCphDelegation()
            {
                CountyParishHoldingId = cph3DeletedButNotExpired.Id,
                DelegatingUserId = delegatingUser1NotDeleted.Id,
                DelegatedUserEmail = delegatedUser2NotDeleted.EmailAddress.ToUpperInvariant(),
                DelegatedUserRoleId = delegateRole1.Id,
            };

        var validatorContext = MockValidatorContext<CreateCphDelegation>.CreateFor(createCphDelegationValidator);

        MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData(
            [
                delegatingUser1NotDeleted,
                delegatedUser2NotDeleted
            ]);

        MockRepositoryContext<Roles>.CreateFor(roleRepository)
            .WithData([delegateRole1]);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(cphRepository)
            .WithData([cph3DeletedButNotExpired]);

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding must exist and not be deleted or expired");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing create county parish holding delegation [county parish holding delegation] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute create county parish holding delegation [county parish holding delegation] failed reference rule 'County parish holding must exist and not be deleted or expired'");
    }

    [Fact]
    [Description("Create throws not found exception when referenced cph does not exist")]
    public async Task Create_Throws_NotFound_Exception_When_Referenced_Cph_Does_Not_Exist()
    {
        // Arrange
        var delegatingUser1NotDeleted = TestData.User.User1NotDeleted;
        var delegatedUser2NotDeleted = TestData.User.User2NotDeleted;
        var delegateRole1 = TestData.Role.Role1;

        var request =
            new CreateCphDelegation()
            {
                CountyParishHoldingId = Guid.NewGuid(),
                DelegatingUserId = delegatingUser1NotDeleted.Id,
                DelegatedUserEmail = delegatedUser2NotDeleted.EmailAddress.ToUpperInvariant(),
                DelegatedUserRoleId = delegateRole1.Id,
            };

        var validatorContext = MockValidatorContext<CreateCphDelegation>.CreateFor(createCphDelegationValidator);

        MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData(
            [
                delegatingUser1NotDeleted,
                delegatedUser2NotDeleted
            ]);

        MockRepositoryContext<Roles>.CreateFor(roleRepository)
            .WithData([delegateRole1]);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(cphRepository);

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding must exist and not be deleted or expired");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing create county parish holding delegation [county parish holding delegation] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute create county parish holding delegation [county parish holding delegation] failed reference rule 'County parish holding must exist and not be deleted or expired'");
    }

    [Fact]
    [Description("Create throws not found exception when referenced delegating user is deleted")]
    public async Task Create_Throws_NotFound_Exception_When_Referenced_Delegating_User_Deleted()
    {
        // Arrange
        var delegatingUser3Deleted = TestData.User.User3Deleted;
        var delegatedUser2NotDeleted = TestData.User.User2NotDeleted;
        var cph1WithAllowedSpeciesNotExpiredOrDeleted = TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted;
        var delegateRole1 = TestData.Role.Role1;

        var request =
            new CreateCphDelegation()
            {
                CountyParishHoldingId = cph1WithAllowedSpeciesNotExpiredOrDeleted.Id,
                DelegatingUserId = delegatingUser3Deleted.Id,
                DelegatedUserEmail = delegatedUser2NotDeleted.EmailAddress.ToUpperInvariant(),
                DelegatedUserRoleId = delegateRole1.Id,
            };

        var validatorContext = MockValidatorContext<CreateCphDelegation>.CreateFor(createCphDelegationValidator);

        MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData(
            [
                delegatingUser3Deleted,
                delegatedUser2NotDeleted
            ]);

        MockRepositoryContext<Roles>.CreateFor(roleRepository)
            .WithData([delegateRole1]);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(cphRepository)
            .WithData([cph1WithAllowedSpeciesNotExpiredOrDeleted]);

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("Delegating user must exist and not be deleted");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing create county parish holding delegation [county parish holding delegation] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute create county parish holding delegation [county parish holding delegation] failed reference rule 'Delegating user must exist and not be deleted'");
    }

    [Fact]
    [Description("Create throws not found exception when referenced delegating user does not exist")]
    public async Task Create_Throws_NotFound_Exception_When_Referenced_Delegating_User_Does_Not_Exist()
    {
        // Arrange
        var delegatedUser2NotDeleted = TestData.User.User2NotDeleted;
        var cph1WithAllowedSpeciesNotExpiredOrDeleted = TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted;
        var delegateRole1 = TestData.Role.Role1;

        var request =
            new CreateCphDelegation()
            {
                CountyParishHoldingId = cph1WithAllowedSpeciesNotExpiredOrDeleted.Id,
                DelegatingUserId = Guid.NewGuid(),
                DelegatedUserEmail = delegatedUser2NotDeleted.EmailAddress.ToUpperInvariant(),
                DelegatedUserRoleId = delegateRole1.Id,
            };

        var validatorContext = MockValidatorContext<CreateCphDelegation>.CreateFor(createCphDelegationValidator);

        MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData([delegatedUser2NotDeleted]);

        MockRepositoryContext<Roles>.CreateFor(roleRepository)
            .WithData([delegateRole1]);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(cphRepository)
            .WithData([cph1WithAllowedSpeciesNotExpiredOrDeleted]);

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("Delegating user must exist and not be deleted");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing create county parish holding delegation [county parish holding delegation] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute create county parish holding delegation [county parish holding delegation] failed reference rule 'Delegating user must exist and not be deleted'");
    }

    [Fact]
    [Description("Create throws not found exception when referenced delegated user is deleted")]
    public async Task Create_Throws_NotFound_Exception_When_Referenced_Delegated_User_Deleted()
    {
        // Arrange
        var delegatingUser1NotDeleted = TestData.User.User1NotDeleted;
        var delegatedUser3Deleted = TestData.User.User3Deleted;
        var cph1WithAllowedSpeciesNotExpiredOrDeleted = TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted;
        var delegateRole1 = TestData.Role.Role1;

        var request =
            new CreateCphDelegation()
            {
                CountyParishHoldingId = cph1WithAllowedSpeciesNotExpiredOrDeleted.Id,
                DelegatingUserId = delegatingUser1NotDeleted.Id,
                DelegatedUserEmail = delegatedUser3Deleted.EmailAddress.ToUpperInvariant(),
                DelegatedUserRoleId = delegateRole1.Id,
            };

        var validatorContext = MockValidatorContext<CreateCphDelegation>.CreateFor(createCphDelegationValidator);

        MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData(
            [
                delegatingUser1NotDeleted,
                delegatedUser3Deleted
            ]);

        MockRepositoryContext<Roles>.CreateFor(roleRepository)
            .WithData([delegateRole1]);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(cphRepository)
            .WithData([cph1WithAllowedSpeciesNotExpiredOrDeleted]);

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("Delegated user must exist and not be deleted");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(0);

        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute create county parish holding delegation [county parish holding delegation] failed reference rule 'Delegated user must exist and not be deleted'");
    }

    [Fact]
    [Description("Create throws not found exception when referenced delegated user is does not exist")]
    public async Task Create_Throws_NotFound_Exception_When_Referenced_Delegated_User_Does_Not_Exist()
    {
        // Arrange
        var delegatingUser1NotDeleted = TestData.User.User1NotDeleted;
        var cph1WithAllowedSpeciesNotExpiredOrDeleted = TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted;
        var delegateRole1 = TestData.Role.Role1;

        var request =
            new CreateCphDelegation()
            {
                CountyParishHoldingId = cph1WithAllowedSpeciesNotExpiredOrDeleted.Id,
                DelegatingUserId = delegatingUser1NotDeleted.Id,
                DelegatedUserEmail = "nonexistentuser@test.com",
                DelegatedUserRoleId = delegateRole1.Id,
            };

        var validatorContext = MockValidatorContext<CreateCphDelegation>.CreateFor(createCphDelegationValidator);

        MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData(
            [
                delegatingUser1NotDeleted,
            ]);

        MockRepositoryContext<Roles>.CreateFor(roleRepository)
            .WithData([delegateRole1]);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(cphRepository)
            .WithData([cph1WithAllowedSpeciesNotExpiredOrDeleted]);

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("Delegated user must exist and not be deleted");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(0);

        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute create county parish holding delegation [county parish holding delegation] failed reference rule 'Delegated user must exist and not be deleted'");
    }

    [Fact]
    [Description("Create throws not found exception when referenced delegated role does not exist")]
    public async Task Create_Throws_NotFound_Exception_When_Referenced_Delegated_Role_Does_Not_Exist()
    {
        // Arrange
        var delegatingUser1NotDeleted = TestData.User.User1NotDeleted;
        var delegatedUser2NotDeleted = TestData.User.User2NotDeleted;
        var cph1WithAllowedSpeciesNotExpiredOrDeleted = TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted;

        var request =
            new CreateCphDelegation()
            {
                CountyParishHoldingId = cph1WithAllowedSpeciesNotExpiredOrDeleted.Id,
                DelegatingUserId = delegatingUser1NotDeleted.Id,
                DelegatedUserEmail = delegatedUser2NotDeleted.EmailAddress.ToUpperInvariant(),
                DelegatedUserRoleId = Guid.NewGuid(),
            };

        var validatorContext = MockValidatorContext<CreateCphDelegation>.CreateFor(createCphDelegationValidator);

        MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData(
            [
                delegatingUser1NotDeleted,
                delegatedUser2NotDeleted
            ]);

        MockRepositoryContext<Roles>.CreateFor(roleRepository);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(cphRepository)
            .WithData([cph1WithAllowedSpeciesNotExpiredOrDeleted]);

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("Delegated user role must exist");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing create county parish holding delegation [county parish holding delegation] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute create county parish holding delegation [county parish holding delegation] failed reference rule 'Delegated user role must exist'");
    }

    [Fact]
    [Description("Create throws validation exception when request validation fails")]
    public async Task Create_Throws_Validation_Exception_When_Request_Validation_Fails()
    {
        // Arrange
        var delegatedUser2NotDeleted = TestData.User.User2NotDeleted;

        var request =
            new CreateCphDelegation()
            {
                CountyParishHoldingId = Guid.NewGuid(),
                DelegatingUserId = Guid.NewGuid(),
                DelegatedUserEmail = delegatedUser2NotDeleted.EmailAddress.ToUpperInvariant(),
                DelegatedUserRoleId = Guid.NewGuid(),
            };

        var validatorContext = MockValidatorContext<CreateCphDelegation>.CreateFor(createCphDelegationValidator)
            .WithValidationFailures(
            [
                new ValidationFailure("Random Property 1", "Simulated validation failure 1"),
                new ValidationFailure("Random Property 2", "Simulated validation failure 2"),
            ]);

        MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData([delegatedUser2NotDeleted]);

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<ValidationException>();

        exception.Message.ShouldContain("Random Property 1: Simulated validation failure 1");
        exception.Message.ShouldContain("Random Property 2: Simulated validation failure 2");

        exception.Errors.Count().ShouldBe(2);

        exception.Errors.ToList().ShouldSatisfyAllConditions(errors =>
        {
            errors[0].PropertyName.ShouldBe("Random Property 1");
            errors[0].ErrorMessage.ShouldBe("Simulated validation failure 1");
            errors[1].PropertyName.ShouldBe("Random Property 2");
            errors[1].ErrorMessage.ShouldBe("Simulated validation failure 2");
        });

        exception.Errors.First().PropertyName.ShouldBe("Random Property 1");
        exception.Errors.First().ErrorMessage.ShouldBe("Simulated validation failure 1");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing create county parish holding delegation [county parish holding delegation] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute create county parish holding delegation [county parish holding delegation] failed validation");
    }

    [Fact]
    [Description("Create throws invalid operation exception when operator context is not provided")]
    public async Task Create_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var delegatedUser2NotDeleted = TestData.User.User2NotDeleted;

        var request =
            new CreateCphDelegation()
            {
                CountyParishHoldingId = Guid.NewGuid(),
                DelegatingUserId = Guid.NewGuid(),
                DelegatedUserEmail = delegatedUser2NotDeleted.EmailAddress.ToUpperInvariant(),
                DelegatedUserRoleId = Guid.NewGuid(),
            };

        var validatorContext = MockValidatorContext<CreateCphDelegation>.CreateFor(createCphDelegationValidator);

        MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData([delegatedUser2NotDeleted]);

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorContext.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidOperationException>();

        exception.Message.ShouldContain("Operator context must be provided for this operation");

        validatorContext.Calls.ShouldHaveNoCalls();
        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);
    }

    [Fact]
    [Description("Create throws unauthorised access exception when operator id is not provided")]
    public async Task Create_Throws_Unauthorised_Access_Exception_When_Operator_Id_Not_Provided()
    {
        // Arrange
        var delegatedUser2NotDeleted = TestData.User.User2NotDeleted;

        var request =
            new CreateCphDelegation()
            {
                CountyParishHoldingId = Guid.NewGuid(),
                DelegatingUserId = Guid.NewGuid(),
                DelegatedUserEmail = delegatedUser2NotDeleted.EmailAddress.ToUpperInvariant(),
                DelegatedUserRoleId = Guid.NewGuid(),
            };

        var validatorContext = MockValidatorContext<CreateCphDelegation>.CreateFor(createCphDelegationValidator);

        MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData([delegatedUser2NotDeleted]);

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();

        exception.Message.ShouldContain("Operator id must be provided for this operation");

        validatorContext.Calls.ShouldHaveNoCalls();
        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);
    }

    [Fact]
    [Description("Accept accepts the delegation invitation and sends email message")]
    public async Task Accept_Accepts_Delegation_Invitation_And_Sends_Email_Message()
    {
        // Arrange
        var delegation = TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
            .ToCph1ValidPendingWithinInviteExpiry;

        var originalDelegation = TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
            .ToCph1ValidPendingWithinInviteExpiry;

        var request = new AcceptCphDelegationById() { Id = delegation.Id };

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository)
            .WithData([delegation]);

        // Act
        await sut.WithOperatorIdOf(TestData.User.User2NotDeleted.Id)
            .Accept(request, TestContext.Current.CancellationToken);

        // Assert
        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(delegation);

        EntityComparer.CreateFor(originalDelegation, delegation)
            .VariancesAtTopLevelWithoutObjectsOrEnumerables
            .ShouldOnlyHaveChanged(nameof(CountyParishHoldingDelegations.InvitationAcceptedAt));

        repositoryContext.Calls.LastUpdateResult.ShouldSatisfyAllConditions(acceptedEntity =>
        {
            acceptedEntity.InvitationAcceptedAt.ShouldNotBeNull();
            acceptedEntity.InvitationAcceptedAt.Value.ShouldBeCloseToUtcNow();
        });

        messagingFactory.ShouldSatisfyAllConditions(
            Assertions.ShouldHaveQueuedDelegationEmailMessage(
                request.Id,
                TestData.User.User1NotDeleted.EmailAddress,
                MessageTemplateTypes.Delegation.DelegationInviterConfirmation));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing accept county parish holding delegation invite [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed accept county parish holding delegation invite [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");
    }

    public static IEnumerable<TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>>
        AcceptDelegationInvalidDelegationStatuses =>
        new List<TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>>
        {
            new TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingInviteExpired,
                TestData.User.User2NotDeleted,
                "Invitation must not have expired"),
            new TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph4ValidAcceptedWithinInviteExpiry,
                TestData.User.User2NotDeleted,
                "Invitation must not have been accepted"),
            new TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph9ValidRejected,
                TestData.User.User2NotDeleted,
                "Invitation must not have been rejected"),
            new TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationRevoked,
                TestData.User.User2NotDeleted,
                "Delegation must not have been revoked"),
            new TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationExpired,
                TestData.User.User2NotDeleted,
                "Delegation must not have expired"),
            new TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationDeleted,
                TestData.User.User2NotDeleted,
                "Delegation must not be deleted"),
        };

    [Theory]
    [MemberData(nameof(AcceptDelegationInvalidDelegationStatuses))]
    [Description("Accept throws business rule exception when the delegation is not in a valid status")]
    public async Task Accept_Throws_Business_Rule_Exception_When_Delegation_Status_Is_Invalid(
        CountyParishHoldingDelegations delegationEntity, UserAccounts delegatedUserEntity, string ruleDescription)
    {
        // Arrange
        var request = new AcceptCphDelegationById() { Id = delegationEntity.Id };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository)
                .WithData([delegationEntity]);

        // Act
        var act = async () =>
            await sut.WithOperatorIdOf(delegatedUserEntity.Id).Accept(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<BusinessRuleException>();

        exception.Message.ShouldContain(ruleDescription);

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldNotBeNull();
        repositoryContext.Calls.LastGetResult.ShouldBe(delegationEntity);
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing accept county parish holding delegation invite [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute accept county parish holding delegation invite [county parish holding delegation] with id {request.Id} failed business rule '{ruleDescription}'");
    }

    [Fact]
    [Description("Accept throws not found exception when delegation does not exist")]
    public async Task Accept_Throws_NotFound_Exception_When_Delegation_Does_Not_Exist()
    {
        // Arrange
        var request = new AcceptCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () => await sut.WithOperatorId.Accept(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding delegation not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing accept county parish holding delegation invite [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"County parish holding delegation with id {request.Id} not found");
    }

    [Fact]
    [Description("Accept throws unauthorised access exception when operator does not match delegated user")]
    public async Task Accept_Throws_Unauthorised_Access_Exception_When_Operator_Does_Not_Match_Delegated_User()
    {
        // Arrange
        var delegation = TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
            .ToCph1ValidPendingWithinInviteExpiry;

        var request = new AcceptCphDelegationById() { Id = delegation.Id };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository)
                .WithData([delegation]);

        // Act
        var act = async () =>
            await sut.WithOperatorIdOf(Guid.NewGuid()).Accept(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();
        exception.Message.ShouldBe("Operator cannot accept or reject this delegation");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(delegation);
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing accept county parish holding delegation invite [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Delegation accept or reject invitation failed authorisation rules for delegation with id {request.Id} and operator {operatorContext!.OperatorId}");
    }

    [Fact]
    [Description("Accept throws invalid operation exception when operator context is not provided")]
    public async Task Accept_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new AcceptCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorContext.Accept(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidOperationException>();

        exception.Message.ShouldContain("Operator context must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);
    }

    [Fact]
    [Description("Accept throws unauthorised access exception when operator id is not provided")]
    public async Task Accept_Throws_Unauthorised_Access_Exception_When_Operator_Id_Not_Provided()
    {
        // Arrange
        var request = new AcceptCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorId.Accept(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();

        exception.Message.ShouldContain("Operator id must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);
    }

    [Fact]
    [Description("Reject rejects the delegation invitation and sends email message")]
    public async Task Reject_Rejects_Delegation_Invitation_And_Sends_Email_Message()
    {
        // Arrange
        var delegation = TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
            .ToCph1ValidPendingWithinInviteExpiry;

        var originalDelegation = TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
            .ToCph1ValidPendingWithinInviteExpiry;

        var request = new RejectCphDelegationById() { Id = delegation.Id };

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository)
            .WithData([delegation]);

        // Act
        await sut.WithOperatorIdOf(TestData.User.User2NotDeleted.Id)
            .Reject(request, TestContext.Current.CancellationToken);

        // Assert
        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(delegation);

        EntityComparer.CreateFor(originalDelegation, delegation)
            .VariancesAtTopLevelWithoutObjectsOrEnumerables
            .ShouldOnlyHaveChanged(nameof(CountyParishHoldingDelegations.InvitationRejectedAt));

        repositoryContext.Calls.LastUpdateResult.ShouldSatisfyAllConditions(rejectedEntity =>
        {
            rejectedEntity.InvitationRejectedAt.ShouldNotBeNull();
            rejectedEntity.InvitationRejectedAt.Value.ShouldBeCloseToUtcNow();
        });

        messagingFactory.ShouldSatisfyAllConditions(
            Assertions.ShouldHaveQueuedDelegationEmailMessage(
                request.Id,
                TestData.User.User1NotDeleted.EmailAddress,
                MessageTemplateTypes.Delegation.DelegationInviterConfirmation));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing reject county parish holding delegation invite [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed reject county parish holding delegation invite [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");
    }

    public static IEnumerable<TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>>
        RejectDelegationInvalidDelegationStatuses =>
        new List<TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>>
        {
            new TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingInviteExpired,
                TestData.User.User2NotDeleted,
                "Invitation must not have expired"),
            new TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph4ValidAcceptedWithinInviteExpiry,
                TestData.User.User2NotDeleted,
                "Invitation must not have been accepted"),
            new TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph9ValidRejected,
                TestData.User.User2NotDeleted,
                "Invitation must not have been rejected"),
            new TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationRevoked,
                TestData.User.User2NotDeleted,
                "Delegation must not have been revoked"),
            new TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationExpired,
                TestData.User.User2NotDeleted,
                "Delegation must not have expired"),
            new TheoryDataRow<CountyParishHoldingDelegations, UserAccounts, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationDeleted,
                TestData.User.User2NotDeleted,
                "Delegation must not be deleted"),
        };

    [Theory]
    [MemberData(nameof(RejectDelegationInvalidDelegationStatuses))]
    [Description("Reject throws business rule exception when the delegation is not in a valid status")]
    public async Task Reject_Throws_Business_Rule_Exception_When_Delegation_Status_Is_Invalid(
        CountyParishHoldingDelegations delegationEntity, UserAccounts delegatedUserEntity, string ruleDescription)
    {
        // Arrange
        var request = new RejectCphDelegationById() { Id = delegationEntity.Id };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository)
                .WithData([delegationEntity]);

        // Act
        var act = async () =>
            await sut.WithOperatorIdOf(delegatedUserEntity.Id).Reject(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<BusinessRuleException>();

        exception.Message.ShouldContain(ruleDescription);

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldNotBeNull();
        repositoryContext.Calls.LastGetResult.ShouldBe(delegationEntity);
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing reject county parish holding delegation invite [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute reject county parish holding delegation invite [county parish holding delegation] with id {request.Id} failed business rule '{ruleDescription}'");
    }

    [Fact]
    [Description("Reject throws not found exception when delegation does not exist")]
    public async Task Reject_Throws_NotFound_Exception_When_Delegation_Does_Not_Exist()
    {
        // Arrange
        var request = new RejectCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () => await sut.WithOperatorId.Reject(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding delegation not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing reject county parish holding delegation invite [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"County parish holding delegation with id {request.Id} not found");
    }

    [Fact]
    [Description("Reject throws unauthorised access exception when operator does not match delegated user")]
    public async Task Reject_Throws_Unauthorised_Access_Exception_When_Operator_Does_Not_Match_Delegated_User()
    {
        // Arrange
        var delegation = TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
            .ToCph1ValidPendingWithinInviteExpiry;

        var request = new RejectCphDelegationById() { Id = delegation.Id };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository)
                .WithData([delegation]);

        // Act
        var act = async () =>
            await sut.WithOperatorIdOf(Guid.NewGuid()).Reject(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();
        exception.Message.ShouldBe("Operator cannot accept or reject this delegation");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(delegation);
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing reject county parish holding delegation invite [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Delegation accept or reject invitation failed authorisation rules for delegation with id {request.Id} and operator {operatorContext!.OperatorId}");
    }

    [Fact]
    [Description("Reject throws invalid operation exception when operator context is not provided")]
    public async Task Reject_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new RejectCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorContext.Reject(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidOperationException>();

        exception.Message.ShouldContain("Operator context must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);
    }

    [Fact]
    [Description("Reject throws unauthorised access exception when operator id is not provided")]
    public async Task Reject_Throws_Unauthorised_Access_Exception_When_Operator_Id_Not_Provided()
    {
        // Arrange
        var request = new RejectCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorId.Reject(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();

        exception.Message.ShouldContain("Operator id must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();

        messagingFactory.ReceivedCalls().Count().ShouldBe(0);
    }

    [Fact]
    [Description("Revoke revokes the delegation")]
    public async Task Revoke_Revokes_Delegation()
    {
        // Arrange
        var delegation = TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
            .ToCph1ValidPendingWithinInviteExpiry;

        var originalDelegation = TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
            .ToCph1ValidPendingWithinInviteExpiry;

        var request = new RevokeCphDelegationById() { Id = delegation.Id };

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository)
            .WithData([delegation]);

        // Act
        await sut.WithOperatorId.Revoke(request, TestContext.Current.CancellationToken);

        // Assert
        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(delegation);

        EntityComparer.CreateFor(originalDelegation, delegation)
            .VariancesAtTopLevelWithoutObjectsOrEnumerables
            .ShouldOnlyHaveChanged(
                nameof(CountyParishHoldingDelegations.RevokedById),
                nameof(CountyParishHoldingDelegations.RevokedAt));

        repositoryContext.Calls.LastUpdateResult.ShouldSatisfyAllConditions(revokedEntity =>
        {
            revokedEntity.RevokedById.ShouldNotBeNull();
            revokedEntity.RevokedById.Value.ShouldBe(operatorContext!.OperatorId);
            revokedEntity.RevokedAt.ShouldNotBeNull();
            revokedEntity.RevokedAt.Value.ShouldBeCloseToUtcNow();
        });

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing revoke county parish holding delegation [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed revoke county parish holding delegation [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");
    }

    public static IEnumerable<TheoryDataRow<CountyParishHoldingDelegations, string>>
        RevokeDelegationInvalidDelegationStatuses =>
        new List<TheoryDataRow<CountyParishHoldingDelegations, string>>
        {
            new TheoryDataRow<CountyParishHoldingDelegations, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationRevoked,
                "Delegation must not have been revoked"),
            new TheoryDataRow<CountyParishHoldingDelegations, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationDeleted,
                "Delegation must not be deleted"),
        };

    [Theory]
    [MemberData(nameof(RevokeDelegationInvalidDelegationStatuses))]
    [Description("Revoke throws business rule exception when the delegation is not in a valid status")]
    public async Task Revoke_Throws_Business_Rule_Exception_When_Delegation_Status_Is_Invalid(
        CountyParishHoldingDelegations delegationEntity, string ruleDescription)
    {
        // Arrange
        var request = new RevokeCphDelegationById() { Id = delegationEntity.Id };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository)
                .WithData([delegationEntity]);

        // Act
        var act = async () =>
            await sut.WithOperatorId.Revoke(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<BusinessRuleException>();

        exception.Message.ShouldContain(ruleDescription);

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldNotBeNull();
        repositoryContext.Calls.LastGetResult.ShouldBe(delegationEntity);
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing revoke county parish holding delegation [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute revoke county parish holding delegation [county parish holding delegation] with id {request.Id} failed business rule '{ruleDescription}'");
    }

    [Fact]
    [Description("Revoke throws not found exception when delegation does not exist")]
    public async Task Revoke_Throws_NotFound_Exception_When_Delegation_Does_Not_Exist()
    {
        // Arrange
        var request = new RevokeCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () => await sut.WithOperatorId.Revoke(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding delegation not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing revoke county parish holding delegation [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"County parish holding delegation with id {request.Id} not found");
    }

    [Fact]
    [Description("Revoke throws invalid operation exception when operator context is not provided")]
    public async Task Revoke_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new RevokeCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorContext.Revoke(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidOperationException>();

        exception.Message.ShouldContain("Operator context must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Revoke throws unauthorised access exception when operator id is not provided")]
    public async Task Revoke_Throws_Unauthorised_Access_Exception_When_Operator_Id_Not_Provided()
    {
        // Arrange
        var request = new RevokeCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorId.Revoke(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();

        exception.Message.ShouldContain("Operator id must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Expire expires the delegation")]
    public async Task Expire_Expires_Delegation()
    {
        // Arrange
        var delegation = TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
            .ToCph1ValidPendingWithinInviteExpiry;

        var originalDelegation = TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
            .ToCph1ValidPendingWithinInviteExpiry;

        var request = new ExpireCphDelegationById() { Id = delegation.Id };

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository)
            .WithData([delegation]);

        // Act
        await sut.WithOperatorId.Expire(request, TestContext.Current.CancellationToken);

        // Assert
        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(delegation);

        EntityComparer.CreateFor(originalDelegation, delegation)
            .VariancesAtTopLevelWithoutObjectsOrEnumerables
            .ShouldOnlyHaveChanged(
                nameof(CountyParishHoldingDelegations.ExpiresAt));

        repositoryContext.Calls.LastUpdateResult.ShouldSatisfyAllConditions(expiredEntity =>
        {
            expiredEntity.ExpiresAt.ShouldNotBeNull();
            expiredEntity.ExpiresAt.Value.ShouldBeCloseToUtcNow();
        });

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing expire county parish holding delegation [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed expire county parish holding delegation [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");
    }

    public static IEnumerable<TheoryDataRow<CountyParishHoldingDelegations, string>>
        ExpireDelegationInvalidDelegationStatuses =>
        new List<TheoryDataRow<CountyParishHoldingDelegations, string>>
        {
            new TheoryDataRow<CountyParishHoldingDelegations, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationExpired,
                "Delegation must not have expired"),
            new TheoryDataRow<CountyParishHoldingDelegations, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationDeleted,
                "Delegation must not be deleted"),
        };

    [Theory]
    [MemberData(nameof(ExpireDelegationInvalidDelegationStatuses))]
    [Description("Expire throws business rule exception when the delegation is not in a valid status")]
    public async Task Expire_Throws_Business_Rule_Exception_When_Delegation_Status_Is_Invalid(
        CountyParishHoldingDelegations delegationEntity, string ruleDescription)
    {
        // Arrange
        var request = new ExpireCphDelegationById() { Id = delegationEntity.Id };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository)
                .WithData([delegationEntity]);

        // Act
        var act = async () =>
            await sut.WithOperatorId.Expire(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<BusinessRuleException>();

        exception.Message.ShouldContain(ruleDescription);

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldNotBeNull();
        repositoryContext.Calls.LastGetResult.ShouldBe(delegationEntity);
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing expire county parish holding delegation [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute expire county parish holding delegation [county parish holding delegation] with id {request.Id} failed business rule '{ruleDescription}'");
    }

    [Fact]
    [Description("Expire throws not found exception when delegation does not exist")]
    public async Task Expire_Throws_NotFound_Exception_When_Delegation_Does_Not_Exist()
    {
        // Arrange
        var request = new ExpireCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () => await sut.WithOperatorId.Expire(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding delegation not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing expire county parish holding delegation [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"County parish holding delegation with id {request.Id} not found");
    }

    [Fact]
    [Description("Expire throws invalid operation exception when operator context is not provided")]
    public async Task Expire_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new ExpireCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorContext.Expire(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidOperationException>();

        exception.Message.ShouldContain("Operator context must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Expire throws unauthorised access exception when operator id is not provided")]
    public async Task Expire_Throws_Unauthorised_Access_Exception_When_Operator_Id_Not_Provided()
    {
        // Arrange
        var request = new ExpireCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorId.Expire(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();

        exception.Message.ShouldContain("Operator id must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Delete deletes the delegation")]
    public async Task Delete_Deletes_Delegation()
    {
        // Arrange
        var delegation = TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
            .ToCph1ValidPendingWithinInviteExpiry;

        var originalDelegation = TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
            .ToCph1ValidPendingWithinInviteExpiry;

        var request = new DeleteCphDelegationById() { Id = delegation.Id };

        var repositoryContext = MockRepositoryContext<CountyParishHoldingDelegations>
            .CreateFor(cphDelegationsRepository)
            .WithData([delegation]);

        // Act
        await sut.WithOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(delegation);

        EntityComparer.CreateFor(originalDelegation, delegation)
            .VariancesAtTopLevelWithoutObjectsOrEnumerables
            .ShouldOnlyHaveChanged(
                nameof(CountyParishHoldingDelegations.DeletedById),
                nameof(CountyParishHoldingDelegations.DeletedAt));

        repositoryContext.Calls.LastUpdateResult.ShouldSatisfyAllConditions(deletedEntity =>
        {
            deletedEntity.DeletedById.ShouldNotBeNull();
            deletedEntity.DeletedById.Value.ShouldBe(operatorContext!.OperatorId);
            deletedEntity.DeletedAt.ShouldNotBeNull();
            deletedEntity.DeletedAt.Value.ShouldBeCloseToUtcNow();
        });

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing delete county parish holding delegation [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed delete county parish holding delegation [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");
    }

    public static IEnumerable<TheoryDataRow<CountyParishHoldingDelegations, string>>
        DeleteDelegationInvalidDelegationStatuses =>
        new List<TheoryDataRow<CountyParishHoldingDelegations, string>>
        {
            new TheoryDataRow<CountyParishHoldingDelegations, string>(
                TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted.ToCph11ValidPendingDelegationDeleted,
                "Delegation must not be deleted"),
        };

    [Theory]
    [MemberData(nameof(DeleteDelegationInvalidDelegationStatuses))]
    [Description("Delete throws business rule exception when the delegation is not in a valid status")]
    public async Task Delete_Throws_Business_Rule_Exception_When_Delegation_Status_Is_Invalid(
        CountyParishHoldingDelegations delegationEntity, string ruleDescription)
    {
        // Arrange
        var request = new DeleteCphDelegationById() { Id = delegationEntity.Id };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository)
                .WithData([delegationEntity]);

        // Act
        var act = async () =>
            await sut.WithOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<BusinessRuleException>();

        exception.Message.ShouldContain(ruleDescription);

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldNotBeNull();
        repositoryContext.Calls.LastGetResult.ShouldBe(delegationEntity);
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing delete county parish holding delegation [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute delete county parish holding delegation [county parish holding delegation] with id {request.Id} failed business rule '{ruleDescription}'");
    }

    [Fact]
    [Description("Delete throws not found exception when delegation does not exist")]
    public async Task Delete_Throws_NotFound_Exception_When_Delegation_Does_Not_Exist()
    {
        // Arrange
        var request = new DeleteCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () => await sut.WithOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding delegation not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing delete county parish holding delegation [county parish holding delegation] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"County parish holding delegation with id {request.Id} not found");
    }

    [Fact]
    [Description("Delete throws invalid operation exception when operator context is not provided")]
    public async Task Delete_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new DeleteCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorContext.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidOperationException>();

        exception.Message.ShouldContain("Operator context must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Delete throws unauthorised access exception when operator id is not provided")]
    public async Task Delete_Throws_Unauthorised_Access_Exception_When_Operator_Id_Not_Provided()
    {
        // Arrange
        var request = new DeleteCphDelegationById() { Id = Guid.NewGuid() };

        var repositoryContext =
            MockRepositoryContext<CountyParishHoldingDelegations>.CreateFor(cphDelegationsRepository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();

        exception.Message.ShouldContain("Operator id must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();
    }
}
