// <copyright file="ApplicationEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Applications;

using Defra.Identity.Api.Endpoints.Applications;
using Defra.Identity.Requests;
using Defra.Identity.Requests.Applications.Commands.Create;
using Defra.Identity.Requests.Applications.Commands.Update;
using Defra.Identity.Requests.Applications.Queries;
using Defra.Identity.Responses.Applications;
using Defra.Identity.Services.Applications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Shouldly;

public class ApplicationEndpointsTests
{
    private readonly IApplicationService service;
    private readonly CommandRequestHeaders commandHeaders;
    private readonly QueryRequestHeaders queryHeaders;

    public ApplicationEndpointsTests()
    {
        service = Substitute.For<IApplicationService>();
        commandHeaders = new CommandRequestHeaders(Guid.NewGuid(), Guid.NewGuid(), "test-api-key");
        queryHeaders = new QueryRequestHeaders(Guid.NewGuid(), "test-api-key");
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        // Arrange
        var request = new GetApplications();
        var applications = new List<Application> { new() { Id = Guid.NewGuid(), Name = "App1" } };
        service.GetAll(request, Arg.Any<CancellationToken>()).Returns(applications);

        // Act
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
            .GetMethod("GetAll", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [queryHeaders, request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<List<Application>>>();
        ((Ok<List<Application>>)result).Value.ShouldBe(applications);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new GetApplicationById { Id = id };
        var application = new Application { Id = id, Name = "App1" };
        service.Get(request, Arg.Any<CancellationToken>()).Returns(application);

        // Act
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
            .GetMethod("Get", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [queryHeaders, request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<Application>>();
        ((Ok<Application>)result).Value.ShouldBe(application);
    }

    [Fact]
    public async Task Post_ReturnsCreatedAtRoute()
    {
        // Arrange
        var request = new CreateApplication { Name = "New App" };
        var application = new Application { Id = Guid.NewGuid(), Name = "New App" };
        service.Create(request, Arg.Any<CancellationToken>()).Returns(application);

        // Act
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
            .GetMethod("Post", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, request, service])!;

        // Assert
        result.ShouldBeOfType<CreatedAtRoute<Application>>();
        var createdResult = (CreatedAtRoute<Application>)result;
        createdResult.Value.ShouldBe(application);
        createdResult.RouteName.ShouldBe(RouteNames.Applications);
        createdResult.RouteValues!["id"].ShouldBe(application.Id);
        request.OperatorId.ShouldBe(commandHeaders.OperatorId);
    }

    [Fact]
    public async Task Put_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateApplication { Name = "Updated App" };
        var application = new Application { Id = id, Name = "Updated App" };
        service.Update(request, Arg.Any<CancellationToken>()).Returns(application);

        // Act
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
            .GetMethod("Put", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, id, request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<Application>>();
        ((Ok<Application>)result).Value.ShouldBe(application);
        request.Id.ShouldBe(id);
        request.OperatorId.ShouldBe(commandHeaders.OperatorId);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenNullReferenceException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateApplication();
        service.Update(request, Arg.Any<CancellationToken>()).Returns(Task.FromException<Application>(new NullReferenceException("Not found")));

        // Act
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
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
        var request = new UpdateApplication();
        service.Update(request, Arg.Any<CancellationToken>()).Returns(Task.FromException<Application>(new Exception("Error")));

        // Act
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
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
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
            .GetMethod("Delete", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [commandHeaders, id, service])!;

        // Assert
        result.ShouldBeOfType<NoContent>();
        await service.Received(1).Delete(id, commandHeaders.OperatorId, Arg.Any<CancellationToken>());
    }
}
