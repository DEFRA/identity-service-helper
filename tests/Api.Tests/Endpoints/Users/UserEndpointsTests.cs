// <copyright file="UserEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Users;

using Defra.Identity.Api.Endpoints.Users;
using Defra.Identity.Models.Requests.Users.Commands;
using Defra.Identity.Models.Requests.Users.Queries;
using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

public class UserEndpointsTests
{
    private readonly IUserService service = Substitute.For<IUserService>();

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var request = new GetAllUsers { IncludeInactive = "true" };

        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test User",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@test.com",
                Active = true,
            },
        };

        service.GetAll(request, Arg.Any<CancellationToken>()).Returns(users);

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod("GetAllRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<List<User>>>();
        ((Ok<List<User>>)result).Value.ShouldBe(users);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            Email = "testuser@test.com",
            Active = true,
        };

        var request = new GetUserById() { Id = user.Id, };

        service.Get(request, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod(
                "GetByIdRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<User>>();
        ((Ok<User>)result).Value.ShouldBe(user);
    }

    [Fact]
    public async Task Post_ReturnsCreatedAtRoute()
    {
        // Arrange
        var request = new CreateUser()
        {
            DisplayName = "Test User", FirstName = "Test", LastName = "User", Email = "testuser@test.com",
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Active = true,
        };

        service.Create(request, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod("PostRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<CreatedAtRoute<User>>();
        var createdResult = (CreatedAtRoute<User>)result;
        createdResult.Value.ShouldBe(user);
        createdResult.RouteName.ShouldBe("GetUser");
        createdResult.RouteValues["id"].ShouldBe(user.Id);
    }

    [Fact]
    public async Task Put_ReturnsOk()
    {
        // Arrange
        var request = new UpdateUserById()
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            Email = "testuser@test.com",
        };

        var user = new User
        {
            Id = request.Id,
            DisplayName = request.DisplayName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Active = true,
        };

        service.Update(request, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod(
                "PutByIdRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<User>>();
        ((Ok<User>)result).Value.ShouldBe(user);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenNotFoundException()
    {
        // Arrange
        var request = new UpdateUserById()
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            Email = "testuser@test.com",
        };

        service.Update(request, Arg.Any<CancellationToken>()).Throws(new NotFoundException("User not found"));

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod(
                "PutByIdRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<NotFound<string>>();
        ((NotFound<string>)result).Value.ShouldBe("User not found");
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenUnhandledException()
    {
        // Arrange
        var request = new UpdateUserById()
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            Email = "testuser@test.com",
        };

        service.Update(request, Arg.Any<CancellationToken>()).Throws(new Exception("A problem occurred"));

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod(
                "PutByIdRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        ((BadRequest<string>)result).Value.ShouldBe("A problem occurred");
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        var request = new DeleteUserById() { Id = Guid.NewGuid(), };

        // Act
        var result = await (Task<IResult>)typeof(UsersEndpoints)
            .GetMethod(
                "DeleteByIdRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<NoContent>();
        await service.Received(1).Delete(Arg.Any<DeleteUserById>(), Arg.Any<CancellationToken>());
    }
}
