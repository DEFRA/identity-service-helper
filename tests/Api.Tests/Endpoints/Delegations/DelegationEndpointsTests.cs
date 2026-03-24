// <copyright file="DelegationEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Delegations;

using Defra.Identity.Api.Endpoints.Delegations;
using Defra.Identity.Requests;
using Defra.Identity.Requests.Delegations.Commands.Create;
using Defra.Identity.Requests.Delegations.Commands.Update;
using Defra.Identity.Requests.Delegations.Queries;
using Defra.Identity.Responses.Delegations;
using Defra.Identity.Services.Delegations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Shouldly;
using Xunit;

public class DelegationEndpointsTests
{
    private readonly ICphDelegationsService service;
    private readonly CommandRequestHeaders commandHeaders;
    private readonly QueryRequestHeaders queryHeaders;

    public DelegationEndpointsTests()
    {
        service = Substitute.For<ICphDelegationsService>();
        commandHeaders = new CommandRequestHeaders(Guid.NewGuid(), Guid.NewGuid(), "test-api-key");
        queryHeaders = new QueryRequestHeaders(Guid.NewGuid(), "test-api-key");
    }

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
                DelegatedUserName = "Test User 200",
                DelegatedUserEmail = "test200@test.com",
                DelegatedUserRoleId = Guid.NewGuid(),
                DelegatedUserRoleName = "Test Role 100",
            },
        };

        service.GetAll(request, Arg.Any<CancellationToken>()).Returns(delegations);

        // Act
        var result = await (Task<IResult>)typeof(DelegationEndpoints)
            .GetMethod("GetAll", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
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
            DelegatedUserName = "Test User 200",
            DelegatedUserEmail = "test200@test.com",
            DelegatedUserRoleId = Guid.NewGuid(),
            DelegatedUserRoleName = "Test Role 100",
        };

        service.Get(request, Arg.Any<CancellationToken>()).Returns(delegation);

        // Act
        var result = await (Task<IResult>)typeof(DelegationEndpoints)
            .GetMethod("Get", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [queryHeaders, request, service])!;

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
            DelegatedUserName = "Test User 200",
            DelegatedUserEmail = request.DelegatedUserEmail,
            DelegatedUserRoleId = request.DelegatedUserRoleId,
            DelegatedUserRoleName = "Test Role 100",
        };

        service.Create(request, Arg.Any<CancellationToken>()).Returns(delegation);

        // Act
        var result = await (Task<IResult>)typeof(DelegationEndpoints)
            .GetMethod("Post", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, request, service])!;

        // Assert
        result.ShouldBeOfType<CreatedAtRoute<CphDelegation>>();
        var createdResult = (CreatedAtRoute<CphDelegation>)result;
        createdResult.Value.ShouldBe(delegation);
        createdResult.RouteName.ShouldBe(RouteNames.Delegations);
        createdResult.RouteValues["id"].ShouldBe(delegation.Id);
        request.OperatorId.ShouldBe(commandHeaders.OperatorId);
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
            DelegatedUserName = "Test User 200",
            DelegatedUserEmail = request.DelegatedUserEmail,
            DelegatedUserRoleId = request.DelegatedUserRoleId,
            DelegatedUserRoleName = "Test Role 100",
        };

        service.Update(request, Arg.Any<CancellationToken>()).Returns(delegation);

        // Act
        var result = await (Task<IResult>)typeof(DelegationEndpoints)
            .GetMethod("Put", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, request.Id, request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<CphDelegation>>();
        ((Ok<CphDelegation>)result).Value.ShouldBe(delegation);
        request.Id.ShouldBe(request.Id);
        request.OperatorId.ShouldBe(commandHeaders.OperatorId);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenNullReferenceException()
    {
        // Arrange
        var request = new UpdateCphDelegationById
        {
            Id = Guid.NewGuid(),
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserEmail = "test200@test.com",
            DelegatedUserRoleId = Guid.NewGuid(),
        };

        service.Update(request, Arg.Any<CancellationToken>()).Returns(Task.FromException<CphDelegation>(new NullReferenceException("Not found")));

        // Act
        var result = await (Task<IResult>)typeof(DelegationEndpoints)
            .GetMethod("Put", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, request.Id, request, service])!;

        // Assert
        result.ShouldBeOfType<NotFound<string>>();
        ((NotFound<string>)result).Value.ShouldBe("Not found");
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenException()
    {
        // Arrange
        var request = new UpdateCphDelegationById
        {
            Id = Guid.NewGuid(),
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserEmail = "test200test.com",
            DelegatedUserRoleId = Guid.NewGuid(),
        };

        service.Update(request, Arg.Any<CancellationToken>()).Returns(Task.FromException<CphDelegation>(new Exception("Error")));

        // Act
        var result = await (Task<IResult>)typeof(DelegationEndpoints)
            .GetMethod("Put", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, request.Id, request, service])!;

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        ((BadRequest<string>)result).Value.ShouldBe("Error");
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        service.Delete(id, commandHeaders.OperatorId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await (Task<IResult>)typeof(DelegationEndpoints)
            .GetMethod("Delete", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, id, service])!;

        // Assert
        result.ShouldBeOfType<NoContent>();
        await service.Received(1).Delete(id, commandHeaders.OperatorId, Arg.Any<CancellationToken>());
    }
}
