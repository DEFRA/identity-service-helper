// <copyright file="UserAccountsEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.UserAccounts;

using Defra.Identity.Api.Endpoints.UserAccounts;
using Defra.Identity.Models.Requests.UserAccounts.Commands;
using Defra.Identity.Models.Responses.UserAccounts;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.UserAccounts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class UserAccountsEndpointsTests
{
    private const string Subject = "9f3a1c2e-0b6d-4f4e-9d2a-7c8b1e5f0a3d";

    private readonly IUserAccountService service = Substitute.For<IUserAccountService>();

    [Fact]
    public async Task Ensure_ReturnsCreatedAtRoute_WhenTheAccountWasCreated()
    {
        // Arrange
        var request = Claims();
        var account = Account();

        this.service
            .EnsureUserAccount(Arg.Any<EnsureUserAccount>(), Arg.Any<CancellationToken>())
            .Returns(new EnsuredUserAccount(account, true));

        // Act
        var result = await InvokeEnsure(request, this.service);

        // Assert
        result.ShouldBeOfType<CreatedAtRoute<UserAccount>>();
        var created = (CreatedAtRoute<UserAccount>)result;
        created.Value.ShouldBe(account);
        created.RouteName.ShouldBe(OpenApiMetadata.GetBySubjectRoute.Name);
        created.RouteValues["sub"].ShouldBe(Subject);
    }

    [Fact]
    public async Task Ensure_ReturnsOk_WhenTheAccountAlreadyExisted()
    {
        // Arrange
        var account = Account();

        this.service
            .EnsureUserAccount(Arg.Any<EnsureUserAccount>(), Arg.Any<CancellationToken>())
            .Returns(new EnsuredUserAccount(account, false));

        // Act
        var result = await InvokeEnsure(Claims(), this.service);

        // Assert
        result.ShouldBeOfType<Ok<UserAccount>>();
        ((Ok<UserAccount>)result).Value.ShouldBe(account);
    }

    [Fact]
    public async Task GetBySubject_ReturnsOk()
    {
        // Arrange
        var account = Account();

        this.service
            .GetUserAccountBySubject(Subject, Arg.Any<CancellationToken>())
            .Returns(account);

        // Act
        var result = await InvokeGetBySubject(Subject, this.service);

        // Assert
        result.ShouldBeOfType<Ok<UserAccount>>();
        ((Ok<UserAccount>)result).Value.ShouldBe(account);
    }

    [Fact]
    public async Task GetBySubject_Propagates_NotFound_ForAnUnknownSubject()
    {
        // Arrange
        this.service
            .GetUserAccountBySubject("unknown", Arg.Any<CancellationToken>())
            .ThrowsAsync(new NotFoundException("User account not found"));

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => InvokeGetBySubject("unknown", this.service));
    }

    private static Task<IResult> InvokeEnsure(EnsureUserAccount request, IUserAccountService service)
    {
        return (Task<IResult>)typeof(UserAccountsEndpoints)
            .GetMethod("EnsureRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service, CancellationToken.None])!;
    }

    private static Task<IResult> InvokeGetBySubject(string sub, IUserAccountService service)
    {
        return (Task<IResult>)typeof(UserAccountsEndpoints)
            .GetMethod(
                "GetBySubjectRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [sub, service, CancellationToken.None])!;
    }

    private static EnsureUserAccount Claims()
    {
        return new EnsureUserAccount
        {
            Sub = Subject,
            Email = "jane.farmer@example.com",
            GivenName = "Jane",
            FamilyName = "Farmer",
        };
    }

    private static UserAccount Account()
    {
        return new UserAccount
        {
            Id = "account-id",
            Subject = Subject,
            Email = "jane.farmer@example.com",
            FirstName = "Jane",
            LastName = "Farmer",
            DisplayName = "Jane Farmer",
            AssociationsRefreshedAt = DateTimeOffset.UtcNow,
            CphAssociations =
            [
                new CphAssociation
                {
                    Id = "cph-identifier-1",
                    CphNumber = "22/001/0001",
                    Role = "owner",
                    PartyId = "party-1",
                    HoldingId = "holding-1",
                    HoldingName = "Hill Farm",
                },
            ],
        };
    }
}
