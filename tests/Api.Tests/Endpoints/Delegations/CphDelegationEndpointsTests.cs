// <copyright file="CphDelegationEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Delegations;

using Defra.Identity.Api.Endpoints.Delegations;
using Defra.Identity.Api.Middleware.Headers;
using Defra.Identity.Models.Requests.Delegations.Commands;
using Defra.Identity.Models.Requests.Delegations.Queries;
using Defra.Identity.Models.Responses.Delegations;
using Defra.Identity.Services.Delegations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Shouldly;
using Xunit;

public class CphDelegationEndpointsTests
{
    private readonly ICphDelegationsService service = Substitute.For<ICphDelegationsService>();

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        // Arrange
        var delegations = new List<CphDelegation>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CountyParishHoldingId = Guid.NewGuid(),
                CountyParishHoldingNumber = "22/001/0001",
                DelegatingUserId = Guid.NewGuid(),
                DelegatingUserName = "Test User 100",
                DelegatedUserId = Guid.NewGuid(),
                DelegatedUserName = "Test User 200",
                DelegatedUserEmail = "test200@test.com",
                DelegatedUserRoleId = Guid.NewGuid(),
                DelegatedUserRoleName = "Test Role 100",
                Active = false,
            },
        };

        service.GetAll(Arg.Any<CancellationToken>()).Returns(delegations);

        // Act
        var result = await (Task<IResult>)typeof(CphDelegationEndpoints)
            .GetMethod("GetAllRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [service])!;

        // Assert
        result.ShouldBeOfType<Ok<List<CphDelegation>>>();
        ((Ok<List<CphDelegation>>)result).Value.ShouldBe(delegations);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new GetCphDelegationById
        {
            Id = id,
        };

        var delegation = new CphDelegation()
        {
            Id = id,
            CountyParishHoldingId = Guid.NewGuid(),
            CountyParishHoldingNumber = "22/001/0001",
            DelegatingUserId = Guid.NewGuid(),
            DelegatingUserName = "Test User 100",
            DelegatedUserId = Guid.NewGuid(),
            DelegatedUserName = "Test User 200",
            DelegatedUserEmail = "test200@test.com",
            DelegatedUserRoleId = Guid.NewGuid(),
            DelegatedUserRoleName = "Test Role 100",
            Active = false,
        };

        service.Get(Arg.Any<GetCphDelegationById>(), Arg.Any<CancellationToken>()).Returns(delegation);

        // Act
        var result = await (Task<IResult>)typeof(CphDelegationEndpoints)
            .GetMethod("GetByIdRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<CphDelegation>>();
        ((Ok<CphDelegation>)result).Value.ShouldBe(delegation);
    }

    [Fact]
    public async Task Post_ReturnsCreatedAtRoute()
    {
        // Arrange
        var request = new CreateCphDelegation
        {
            CountyParishHoldingId = Guid.NewGuid(), DelegatingUserId = Guid.NewGuid(), DelegatedUserEmail = "test200@test.com", DelegatedUserRoleId = Guid.NewGuid(),
        };

        var delegation = new CphDelegation()
        {
            Id = Guid.NewGuid(),
            CountyParishHoldingId = request.CountyParishHoldingId,
            CountyParishHoldingNumber = "22/001/0001",
            DelegatingUserId = request.DelegatingUserId,
            DelegatingUserName = "Test User 100",
            DelegatedUserId = Guid.NewGuid(),
            DelegatedUserName = "Test User 200",
            DelegatedUserEmail = request.DelegatedUserEmail,
            DelegatedUserRoleId = request.DelegatedUserRoleId,
            DelegatedUserRoleName = "Test Role 100",
            Active = false,
        };

        service.Create(request, Arg.Any<CancellationToken>()).Returns(delegation);

        // Act
        var result = await (Task<IResult>)typeof(CphDelegationEndpoints)
            .GetMethod("PostRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<CreatedAtRoute<CphDelegation>>();
        var createdResult = (CreatedAtRoute<CphDelegation>)result;
        createdResult.Value.ShouldBe(delegation);
        createdResult.RouteName.ShouldBe("GetById");
        createdResult.RouteValues["id"].ShouldBe(delegation.Id);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        var request = new DeleteCphDelegationById()
        {
            Id = Guid.NewGuid(),
        };

        // Act
        var result = await (Task<IResult>)typeof(CphDelegationEndpoints)
            .GetMethod("DeleteByIdRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<NoContent>();
        await service.Received(1).Delete(Arg.Any<DeleteCphDelegationById>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AcceptInvitation_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        const string invitationToken = "0000000000000000000000000000000000000000000000000000000000000001";

        // Act
        var result = await (Task<IResult>)typeof(CphDelegationEndpoints)
            .GetMethod("AcceptInvitationByIdRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [id, invitationToken, service])!;

        // Assert
        result.ShouldBeOfType<Ok>();
        await service.Received(1).AcceptInvitation(
            Arg.Is<AcceptInvitationById>(request => request.Id == id && request.InvitationToken == invitationToken),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RejectInvitation_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        const string invitationToken = "0000000000000000000000000000000000000000000000000000000000000001";

        // Act
        var result = await (Task<IResult>)typeof(CphDelegationEndpoints)
            .GetMethod("RejectInvitationByIdRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [id, invitationToken, service])!;

        // Assert
        result.ShouldBeOfType<Ok>();
        await service.Received(1).RejectInvitation(
            Arg.Is<RejectInvitationById>(request => request.Id == id && request.InvitationToken == invitationToken),
            Arg.Any<CancellationToken>());
    }
}
