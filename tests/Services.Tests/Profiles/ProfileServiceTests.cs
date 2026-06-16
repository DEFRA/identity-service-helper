// <copyright file="ProfileServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Profiles;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Profiles.Queries;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Assignments;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Services.Common.Strategy.Factories;
using Defra.Identity.Services.Profiles;
using Defra.Identity.Test.Utilities.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class ProfileServiceTests
{
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly ICphAssignmentsRepository cphAssignmentsRepository = Substitute.For<ICphAssignmentsRepository>();
    private readonly ICphDelegationsRepository cphDelegationsRepository = Substitute.For<ICphDelegationsRepository>();

    private readonly IStrategyBuilderFactory<ProfileService> strategyBuilderFactory =
        new StrategyBuilderFactory<ProfileService>();

    private readonly ILogger<ProfileService> logger =
        DefraLoggerExtensions.CreateNSubstituteLogger<ProfileService>();

    private readonly SutProvider<ProfileService> sut;

    public ProfileServiceTests()
    {
        sut = SutProvider<ProfileService>.CreateFor(_ => new ProfileService(
            userRepository,
            cphAssignmentsRepository,
            cphDelegationsRepository,
            strategyBuilderFactory,
            logger));

        MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData(
            [
                TestData.User.User1NotDeleted,
                TestData.User.User2NotDeleted,
                TestData.User.User3Deleted,
                TestData.User.User4NotDeleted,
                TestData.User.User5NotDeleted,
            ]);

        MockRepositoryContext<UserAccountCountyParishHoldingAssignments>.CreateFor(cphAssignmentsRepository)
            .WithData(
            [
                TestData.CphAssignments.ToUser1NotDeleted.ToCph1Valid,
                TestData.CphAssignments.ToUser1NotDeleted.ToCph2ReferencedCphExpired,
                TestData.CphAssignments.ToUser1NotDeleted.ToCph3ReferencedCphDeleted,
                TestData.CphAssignments.ToUser1NotDeleted.ToCph4Valid,
                TestData.CphAssignments.ToUser1NotDeleted.ToCph7Deleted,
                TestData.CphAssignments.ToUser1NotDeleted.ToCph9Valid,
                TestData.CphAssignments.ToUser1NotDeleted.ToCph10Valid,
                TestData.CphAssignments.ToUser1NotDeleted.ToCph11Valid,
                TestData.CphAssignments.ToUser2NotDeleted.ToCph13Valid,
                TestData.CphAssignments.ToUser2NotDeleted.ToCph14Valid,
                TestData.CphAssignments.ToUser3Deleted.ToCph15Valid,
                TestData.CphAssignments.ToUser3Deleted.ToCph16Valid,
                TestData.CphAssignments.ToUser5NotDeleted.ToCph1Valid,
            ]);

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
    }

    [Fact]
    [Description(
        "GetUserProfile returns the requested profile for user 1 and exludes inactve assignments and delegations")]
    public async Task GetUserProfile_Returns_Requested_User1_Profile_Excludes_Inactive_Assignments_And_Delegations()
    {
        // Arrange
        var request = new GetUserProfileByUserId { Id = TestData.User.User1NotDeleted.Id };

        // Act
        var result = await sut.WithoutOperatorContext.GetUserProfile(request, TestContext.Current.CancellationToken);

        // Assert
        result.UserDetails.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.User.User1NotDeleted));

        result.DirectAssignments.Count().ShouldBe(5);

        var directAssignments = result.DirectAssignments.ToList();

        directAssignments[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphAssignments.ToUser1NotDeleted.ToCph1Valid));

        directAssignments[1]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphAssignments.ToUser1NotDeleted.ToCph4Valid));

        directAssignments[2]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphAssignments.ToUser1NotDeleted.ToCph9Valid));

        directAssignments[3]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphAssignments.ToUser1NotDeleted.ToCph10Valid));

        directAssignments[4]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphAssignments.ToUser1NotDeleted.ToCph11Valid));

        result.InboundDelegations.Count().ShouldBe(2);

        var inboundDelegations = result.InboundDelegations.ToList();

        inboundDelegations[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser2NotDeleted.ToUser1NotDeleted
                    .ToCph13ValidPendingWithinInviteExpiry));

        inboundDelegations[1]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser2NotDeleted.ToUser1NotDeleted
                    .ToCph14ValidAcceptedWithinInviteExpiry));

        result.OutboundDelegations.Count().ShouldBe(5);

        var outboundDelegations = result.OutboundDelegations.ToList();

        outboundDelegations[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
                    .ToCph1ValidPendingWithinInviteExpiry));

        outboundDelegations[1]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
                    .ToCph4ValidAcceptedWithinInviteExpiry));

        outboundDelegations[2]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
                    .ToCph10ValidAcceptedWithinInviteExpiry));

        outboundDelegations[3]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
                    .ToCph11ValidAcceptedInviteNowExpired));

        outboundDelegations[4]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted.ToUser4NotDeleted
                    .ToCph1ValidPendingWithinInviteExpiry));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get user details [user account] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get user details [user account] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get county parish holdings assigned to user [county parish holding assignment]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get county parish holdings assigned to user [county parish holding assignment]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get all county parish holdings delegated to user [county parish holding delegation]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get all county parish holdings delegated to user [county parish holding delegation]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get all county parish holdings delegations associated with cphs owned by user [county parish holding delegation]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get all county parish holdings delegations associated with cphs owned by user [county parish holding delegation]");
    }

    [Fact]
    [Description(
        "GetUserProfile returns the requested profile for user 2 and exludes inactve assignments and delegations")]
    public async Task GetUserProfile_Returns_Requested_User2_Profile_Excludes_Inactive_Assignments_And_Delegations()
    {
        // Arrange
        var request = new GetUserProfileByUserId { Id = TestData.User.User2NotDeleted.Id };

        // Act
        var result = await sut.WithoutOperatorContext.GetUserProfile(request, TestContext.Current.CancellationToken);

        // Assert
        result.UserDetails.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.User.User2NotDeleted));

        result.DirectAssignments.Count().ShouldBe(2);

        var directAssignments = result.DirectAssignments.ToList();

        directAssignments[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphAssignments.ToUser2NotDeleted.ToCph13Valid));

        directAssignments[1]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphAssignments.ToUser2NotDeleted.ToCph14Valid));

        result.InboundDelegations.Count().ShouldBe(4);

        var inboundDelegations = result.InboundDelegations.ToList();

        inboundDelegations[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
                    .ToCph1ValidPendingWithinInviteExpiry));

        inboundDelegations[1]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
                    .ToCph4ValidAcceptedWithinInviteExpiry));

        inboundDelegations[2]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
                    .ToCph10ValidAcceptedWithinInviteExpiry));

        inboundDelegations[3]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
                    .ToCph11ValidAcceptedInviteNowExpired));

        result.OutboundDelegations.Count().ShouldBe(2);

        var outboundDelegations = result.OutboundDelegations.ToList();

        outboundDelegations[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser2NotDeleted.ToUser1NotDeleted
                    .ToCph13ValidPendingWithinInviteExpiry));

        outboundDelegations[1]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser2NotDeleted.ToUser1NotDeleted
                    .ToCph14ValidAcceptedWithinInviteExpiry));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get user details [user account] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get user details [user account] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get county parish holdings assigned to user [county parish holding assignment]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get county parish holdings assigned to user [county parish holding assignment]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get all county parish holdings delegated to user [county parish holding delegation]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get all county parish holdings delegated to user [county parish holding delegation]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get all county parish holdings delegations associated with cphs owned by user [county parish holding delegation]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get all county parish holdings delegations associated with cphs owned by user [county parish holding delegation]");
    }

    [Fact]
    [Description(
        "GetUserProfile returns the requested profile for user 4 and exludes inactve assignments and delegations")]
    public async Task GetUserProfile_Returns_Requested_User4_Profile_Excludes_Inactive_Assignments_And_Delegations()
    {
        // Arrange
        var request = new GetUserProfileByUserId { Id = TestData.User.User4NotDeleted.Id };

        // Act
        var result = await sut.WithoutOperatorContext.GetUserProfile(request, TestContext.Current.CancellationToken);

        // Assert
        result.UserDetails.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.User.User4NotDeleted));

        result.DirectAssignments.Count().ShouldBe(0);
        result.InboundDelegations.Count().ShouldBe(1);

        var inboundDelegations = result.InboundDelegations.ToList();

        inboundDelegations[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted.ToUser4NotDeleted
                    .ToCph1ValidPendingWithinInviteExpiry));

        result.OutboundDelegations.Count().ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get user details [user account] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get user details [user account] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get county parish holdings assigned to user [county parish holding assignment]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get county parish holdings assigned to user [county parish holding assignment]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get all county parish holdings delegated to user [county parish holding delegation]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get all county parish holdings delegated to user [county parish holding delegation]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get all county parish holdings delegations associated with cphs owned by user [county parish holding delegation]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get all county parish holdings delegations associated with cphs owned by user [county parish holding delegation]");
    }

    [Fact]
    [Description(
        "GetUserProfile returns the requested profile for user 5 who is a cph 1 co-asignee and exludes inactve assignments and delegations")]
    public async Task
        GetUserProfile_Returns_Requested_User5_Cph1_CoAssignee_Profile_Excludes_Inactive_Assignments_And_Delegations()
    {
        // Arrange
        var request = new GetUserProfileByUserId { Id = TestData.User.User5NotDeleted.Id };

        // Act
        var result = await sut.WithoutOperatorContext.GetUserProfile(request, TestContext.Current.CancellationToken);

        // Assert
        result.UserDetails.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.User.User5NotDeleted));

        result.DirectAssignments.Count().ShouldBe(1);

        var directAssignments = result.DirectAssignments.ToList();

        directAssignments[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphAssignments.ToUser5NotDeleted.ToCph1Valid));

        result.InboundDelegations.Count().ShouldBe(0);
        result.OutboundDelegations.Count().ShouldBe(2);

        var outboundDelegations = result.OutboundDelegations.ToList();

        outboundDelegations[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted.ToUser2NotDeleted
                    .ToCph1ValidPendingWithinInviteExpiry));

        outboundDelegations[1]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.CphDelegations.FromUser1NotDeleted.ToUser4NotDeleted
                    .ToCph1ValidPendingWithinInviteExpiry));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get user details [user account] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get user details [user account] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get county parish holdings assigned to user [county parish holding assignment]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get county parish holdings assigned to user [county parish holding assignment]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get all county parish holdings delegated to user [county parish holding delegation]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get all county parish holdings delegated to user [county parish holding delegation]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get all county parish holdings delegations associated with cphs owned by user [county parish holding delegation]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get all county parish holdings delegations associated with cphs owned by user [county parish holding delegation]");
    }

    [Fact]
    [Description("GetUserProfile_Throws_NotFound_Exception_When_User_Does_Not_Exist")]
    public async Task GetUserProfile_Throws_NotFound_Exception_When_User_Does_Not_Exist()
    {
        // Arrange
        var request = new GetUserProfileByUserId { Id = Guid.NewGuid() };

        var userRepositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(userRepository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorContext.GetUserProfile(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("User account not found");

        userRepositoryContext.Calls.GetCallCount.ShouldBe(1);
        userRepositoryContext.Calls.LastGetResult.ShouldBeNull();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get user details [user account] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"User account with id {request.Id} not found");
    }

    [Fact]
    [Description("GetUserProfile throws not found exception when application is deleted")]
    public async Task GetUserProfile_Throws_NotFound_Exception_When_User_Deleted()
    {
        // Arrange
        var user3Deleted = TestData.User.User3Deleted;

        var request = new GetUserProfileByUserId { Id = user3Deleted.Id };

        var userRepositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(userRepository)
            .WithData(
            [
                user3Deleted
            ]);

        // Act
        var act = async () =>
            await sut.WithoutOperatorContext.GetUserProfile(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("User account not found");

        // Check that the repository returned the deleted item for evaluation of the deleted state
        userRepositoryContext.Calls.GetCallCount.ShouldBe(1);
        userRepositoryContext.Calls.LastGetResult.ShouldBe(user3Deleted);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get user details [user account] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"User account with id {request.Id} not found");
    }
}
