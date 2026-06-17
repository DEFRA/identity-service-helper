// <copyright file="ProfileEndpointsTest.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Profiles;

using Defra.Identity.Api.Endpoints.Profiles;
using Defra.Identity.Models.Requests.Profiles.Queries;
using Defra.Identity.Models.Responses.Assignments;
using Defra.Identity.Models.Responses.Delegations;
using Defra.Identity.Models.Responses.Profiles;
using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Services.Profiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

public class ProfileEndpointsTest
{
    private readonly IProfileService service = Substitute.For<IProfileService>();

    [Fact]
    public async Task Get_ReturnsOk()
    {
        // Arrange
        var request = new GetUserProfileByUserId() { Id = Guid.NewGuid(), };

        var userProfile = new UserProfile(
            new User
            {
                Id = request.Id,
                DisplayName = "Test User",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@test.com",
                Active = true,
            },
            new List<CphAssignment>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CountyParishHoldingId = Guid.NewGuid(),
                    CountyParishHoldingNumber = "22/001/0001",
                    UserId = request.Id,
                    DisplayName = "Test User",
                    Email = "testuser@test.com",
                    RoleId = Guid.NewGuid(),
                    RoleName = "Test Role",
                },
            },
            new List<CphDelegation>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    DelegatingUserId = request.Id,
                    DelegatingUserName = "Test Delegator",
                    DelegatedUserId = Guid.NewGuid(),
                    DelegatedUserName = "Test Delegated",
                    DelegatedUserEmail = "testdelegated@test.com",
                    CountyParishHoldingId = Guid.NewGuid(),
                    CountyParishHoldingNumber = "22/001/0001",
                    DelegatedUserRoleId = Guid.NewGuid(),
                    DelegatedUserRoleName = "Test Role",
                    Active = true,
                },
            },
            new List<CphDelegation>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    DelegatingUserId = request.Id,
                    DelegatingUserName = "Test Delegator",
                    DelegatedUserId = Guid.NewGuid(),
                    DelegatedUserName = "Test Delegated",
                    DelegatedUserEmail = "testdelegated@test.com",
                    CountyParishHoldingId = Guid.NewGuid(),
                    CountyParishHoldingNumber = "22/001/0001",
                    DelegatedUserRoleId = Guid.NewGuid(),
                    DelegatedUserRoleName = "Test Role",
                    Active = true,
                },
            });

        service.GetUserProfile(Arg.Any<GetUserProfileByUserId>(), Arg.Any<CancellationToken>()).Returns(userProfile);

        // Act
        var result = await (Task<IResult>)typeof(ProfileEndpoints)
            .GetMethod(
                "GetUserProfileByIdRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<UserProfile>>();
        ((Ok<UserProfile>)result).Value.ShouldBe(userProfile);
    }
}
