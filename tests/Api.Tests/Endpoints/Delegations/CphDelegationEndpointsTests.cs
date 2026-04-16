// <copyright file="CphDelegationEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Delegations;

using Defra.Identity.Api.Endpoints.Delegations;
using Defra.Identity.Models.Requests;
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
    private readonly CommandRequestHeaders commandHeaders = new(Guid.NewGuid(), Guid.NewGuid(), "test-api-key");
    private readonly QueryRequestHeaders queryHeaders = new(Guid.NewGuid(), "test-api-key");

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        // Arrange
        var request = new GetCphDelegations();
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
            },
        };

        service.GetAll(request, Arg.Any<CancellationToken>()).Returns(delegations);

        // Act
        var result = await (Task<IResult>)typeof(CphDelegationEndpoints)
            .GetMethod("GetAllRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [queryHeaders, request, service])!;

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
        };

        service.Get(Arg.Any<GetCphDelegationById>(), Arg.Any<CancellationToken>()).Returns(delegation);

        // Act
        var result = await (Task<IResult>)typeof(CphDelegationEndpoints)
            .GetMethod("GetByIdRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [queryHeaders, request.Id, service])!;

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
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserId = Guid.NewGuid(),
            DelegatedUserEmail = "test200@test.com",
            DelegatedUserRoleId = Guid.NewGuid(),
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
        };

        service.Create(request, Arg.Any<CancellationToken>()).Returns(delegation);

        // Act
        var result = await (Task<IResult>)typeof(CphDelegationEndpoints)
            .GetMethod("PostRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, request, service])!;

        // Assert
        result.ShouldBeOfType<CreatedAtRoute<CphDelegation>>();
        var createdResult = (CreatedAtRoute<CphDelegation>)result;
        createdResult.Value.ShouldBe(delegation);
        createdResult.RouteName.ShouldBe(RouteNames.Delegations);
        createdResult.RouteValues["id"].ShouldBe(delegation.Id);
    }

    [Fact]
    public async Task Put_ReturnsOk()
    {
        // Arrange
        var request = new UpdateCphDelegationById
        {
            Id = Guid.NewGuid(),
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserId = Guid.NewGuid(),
            DelegatedUserEmail = "test200@test.com",
            DelegatedUserRoleId = Guid.NewGuid(),
        };

        var delegation = new CphDelegation()
        {
            Id = request.Id,
            CountyParishHoldingId = request.CountyParishHoldingId,
            CountyParishHoldingNumber = "22/001/0001",
            DelegatingUserId = request.DelegatingUserId,
            DelegatingUserName = "Test User 100",
            DelegatedUserId = Guid.NewGuid(),
            DelegatedUserName = "Test User 200",
            DelegatedUserEmail = request.DelegatedUserEmail,
            DelegatedUserRoleId = request.DelegatedUserRoleId,
            DelegatedUserRoleName = "Test Role 100",
        };

        service.Update(request, Arg.Any<CancellationToken>()).Returns(delegation);

        // Act
        var result = await (Task<IResult>)typeof(CphDelegationEndpoints)
            .GetMethod("PutByIdRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, request.Id, request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<CphDelegation>>();
        ((Ok<CphDelegation>)result).Value.ShouldBe(delegation);
        request.Id.ShouldBe(request.Id);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        var request = new DeleteCphDelegationById()
        {
            Id = Guid.NewGuid(),
        };

        service.Delete(Arg.Any<DeleteCphDelegationById>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await (Task<IResult>)typeof(CphDelegationEndpoints)
            .GetMethod("DeleteByIdRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, request.Id, service])!;

        // Assert
        result.ShouldBeOfType<NoContent>();
        await service.Received(1).Delete(Arg.Any<DeleteCphDelegationById>(), Arg.Any<CancellationToken>());
    }
}
