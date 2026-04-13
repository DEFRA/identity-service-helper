// <copyright file="UsersEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Users;

using Defra.Identity.Api.Endpoints.Users;
using Defra.Identity.Requests;
using Defra.Identity.Requests.Users.Commands.Create;
using Defra.Identity.Requests.Users.Commands.Update;
using Defra.Identity.Requests.Users.Commands.Validate;
using Defra.Identity.Requests.Users.Queries;
using Defra.Identity.Responses.Users;
using Defra.Identity.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Shouldly;
using Xunit;

public class UsersEndpointsTests
{
    private readonly IUserService service;
    private readonly CommandRequestHeaders commandHeaders;
    private readonly QueryRequestHeaders queryHeaders;

    public UsersEndpointsTests()
    {
        service = Substitute.For<IUserService>();
        commandHeaders = new CommandRequestHeaders(Guid.NewGuid(), Guid.NewGuid(), "test-api-key");
        queryHeaders = new QueryRequestHeaders(Guid.NewGuid(), "test-api-key");
    }

    [Fact]
    public async Task ValidateUser_ReturnsOk_WhenValid()
    {
        // Arrange
        var request = new ValidateUser { Email = "test@example.com" };
        service.Validate(commandHeaders.OperatorId, request.Email, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod("ValidateUser", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, request, service])!;

        // Assert
        result.ShouldBeOfType<Ok>();
    }

    [Fact]
    public async Task ValidateUser_ReturnsNotFound_WhenInvalid()
    {
        // Arrange
        var request = new ValidateUser { Email = "test@example.com" };
        service.Validate(commandHeaders.OperatorId, request.Email, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod("ValidateUser", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, request, service])!;

        // Assert
        result.ShouldBeOfType<NotFound>();
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        // Arrange
        var request = new GetUsers();
        var users = new List<User> { new() { Id = Guid.NewGuid(), Email = "user1@example.com" } };
        service.GetAll(request, Arg.Any<CancellationToken>()).Returns(users);

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod("GetAll", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [queryHeaders, request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<List<User>>>();
        ((Ok<List<User>>)result).Value.ShouldBe(users);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new GetUserById { Id = id };
        var user = new User { Id = id, Email = "test@example.com" };
        service.Get(request, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod("Get", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [queryHeaders, request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<User>>();
        ((Ok<User>)result).Value.ShouldBe(user);
    }

    [Fact]
    public async Task Post_ReturnsCreatedAtRoute()
    {
        // Arrange
        var request = new CreateUser { Email = "new@example.com" };
        var user = new User { Id = Guid.NewGuid(), Email = "new@example.com" };
        service.Create(request, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod("Post", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, request, service])!;

        // Assert
        result.ShouldBeOfType<CreatedAtRoute<User>>();
        var createdResult = (CreatedAtRoute<User>)result;
        createdResult.Value.ShouldBe(user);
        createdResult.RouteName.ShouldBe(RouteNames.Users);
        createdResult.RouteValues!["id"].ShouldBe(user.Id);
        request.OperatorId.ShouldBe(commandHeaders.OperatorId);
    }

    [Fact]
    public async Task Put_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateUser { Email = "updated@example.com" };
        var user = new User { Id = id, Email = "updated@example.com" };
        service.Update(request, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod("Put", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, id, request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<User>>();
        ((Ok<User>)result).Value.ShouldBe(user);
        request.Id.ShouldBe(id);
        request.OperatorId.ShouldBe(commandHeaders.OperatorId);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        service.Delete(id, commandHeaders.OperatorId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod("Delete", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, id, service])!;

        // Assert
        result.ShouldBeOfType<NoContent>();
        await service.Received(1).Delete(id, commandHeaders.OperatorId, Arg.Any<CancellationToken>());
    }
}
