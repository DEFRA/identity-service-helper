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
    private readonly IDelegationsService service;
    private readonly CommandRequestHeaders commandHeaders;
    private readonly QueryRequestHeaders queryHeaders;

    public DelegationEndpointsTests()
    {
        service = Substitute.For<IDelegationsService>();
        commandHeaders = new CommandRequestHeaders(Guid.NewGuid(), Guid.NewGuid(), "test-api-key");
        queryHeaders = new QueryRequestHeaders(Guid.NewGuid(), "test-api-key");
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        // Arrange
        var request = new GetDelegations();
        var delegations = new List<Delegation> { new() { Id = Guid.NewGuid() } };
        service.GetAll(request, Arg.Any<CancellationToken>()).Returns(delegations);

        // Act
        var result = await (Task<IResult>)typeof(DelegationEndpoints)
            .GetMethod("GetAll", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [queryHeaders, request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<List<Delegation>>>();
        ((Ok<List<Delegation>>)result).Value.ShouldBe(delegations);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new GetDelegationById { Id = id };
        var delegation = new Delegation { Id = id };
        service.Get(request, Arg.Any<CancellationToken>()).Returns(delegation);

        // Act
        var result = await (Task<IResult>)typeof(DelegationEndpoints)
            .GetMethod("Get", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [queryHeaders, request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<Delegation>>();
        ((Ok<Delegation>)result).Value.ShouldBe(delegation);
    }

    [Fact]
    public async Task Post_ReturnsCreatedAtRoute()
    {
        // Arrange
        var request = new CreateDelegation { ApplicationId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        var delegation = new Delegation { Id = Guid.NewGuid() };
        service.Create(request, Arg.Any<CancellationToken>()).Returns(delegation);

        // Act
        var result = await (Task<IResult>)typeof(DelegationEndpoints)
            .GetMethod("Post", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, request, service])!;

        // Assert
        result.ShouldBeOfType<CreatedAtRoute<Delegation>>();
        var createdResult = (CreatedAtRoute<Delegation>)result;
        createdResult.Value.ShouldBe(delegation);
        createdResult.RouteName.ShouldBe(RouteNames.Delegations);
        createdResult.RouteValues!["id"].ShouldBe(delegation.Id);
        request.OperatorId.ShouldBe(commandHeaders.OperatorId);
    }

    [Fact]
    public async Task Put_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateDelegation { ApplicationId = Guid.NewGuid() };
        var delegation = new Delegation { Id = id };
        service.Update(request, Arg.Any<CancellationToken>()).Returns(delegation);

        // Act
        var result = await (Task<IResult>)typeof(DelegationEndpoints)
            .GetMethod("Put", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, id, request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<Delegation>>();
        ((Ok<Delegation>)result).Value.ShouldBe(delegation);
        request.Id.ShouldBe(id);
        request.OperatorId.ShouldBe(commandHeaders.OperatorId);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenNullReferenceException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateDelegation();
        service.Update(request, Arg.Any<CancellationToken>()).Returns(Task.FromException<Delegation>(new NullReferenceException("Not found")));

        // Act
        var result = await (Task<IResult>)typeof(DelegationEndpoints)
            .GetMethod("Put", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, id, request, service])!;

        // Assert
        result.ShouldBeOfType<NotFound<string>>();
        ((NotFound<string>)result).Value.ShouldBe("Not found");
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateDelegation();
        service.Update(request, Arg.Any<CancellationToken>()).Returns(Task.FromException<Delegation>(new Exception("Error")));

        // Act
        var result = await (Task<IResult>)typeof(DelegationEndpoints)
            .GetMethod("Put", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, id, request, service])!;

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
