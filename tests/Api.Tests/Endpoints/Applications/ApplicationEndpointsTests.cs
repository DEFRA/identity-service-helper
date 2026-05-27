// <copyright file="ApplicationEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Applications;

using Defra.Identity.Api.Endpoints.Applications;
using Defra.Identity.Api.Middleware.Headers;
using Defra.Identity.Models.Requests.Applications.Commands;
using Defra.Identity.Models.Requests.Applications.Queries;
using Defra.Identity.Models.Responses.Applications;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.Applications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;

public class ApplicationEndpointsTests
{
    private readonly IApplicationService service = Substitute.For<IApplicationService>();

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        // Arrange
        var request = new GetApplications();
        var applications = new List<Application>
        {
            new()
            {
                Id = Guid.NewGuid(), Name = "App1",
            },
        };
        service.GetAll(request, Arg.Any<CancellationToken>()).Returns(applications);

        // Act
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
            .GetMethod("GetAllRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<List<Application>>>();
        ((Ok<List<Application>>)result).Value.ShouldBe(applications);
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new GetApplicationByClientId
        {
            Id = id,
        };
        var application = new Application
        {
            Id = id, Name = "App1",
        };
        service.Get(Arg.Any<GetApplicationByClientId>(), Arg.Any<CancellationToken>()).Returns(application);

        // Act
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
            .GetMethod("GetByIdRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<Application>>();
        ((Ok<Application>)result).Value.ShouldBe(application);
    }

    [Fact]
    public async Task Post_ReturnsCreatedAtRoute()
    {
        // Arrange
        var request = new CreateApplication
        {
            Name = "New App",
        };
        var application = new Application
        {
            Id = Guid.NewGuid(), Name = "New App",
        };
        service.Create(request, Arg.Any<CancellationToken>()).Returns(application);

        // Act
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
            .GetMethod("PostRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<CreatedAtRoute<Application>>();
        var createdResult = (CreatedAtRoute<Application>)result;
        createdResult.Value.ShouldBe(application);
        createdResult.RouteName.ShouldBe("GetApplication");
        createdResult.RouteValues["id"].ShouldBe(application.Id);
    }

    [Fact]
    public async Task Put_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new UpdateApplicationByClientId
        {
            Id = id, Name = "Updated App",
        };

        var application = new Application
        {
            Id = id, Name = "Updated App",
        };

        service.Update(request, Arg.Any<CancellationToken>()).Returns(application);

        // Act
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
            .GetMethod("PutByIdRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<Ok<Application>>();
        ((Ok<Application>)result).Value.ShouldBe(application);
        request.Id.ShouldBe(id);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new UpdateApplicationByClientId()
        {
            Id = id,
        };

        service.Update(request, Arg.Any<CancellationToken>()).Returns(Task.FromException<Application>(new NotFoundException("Not found")));

        // Act
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
            .GetMethod("PutByIdRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<NotFound<string>>();
        ((NotFound<string>)result).Value.ShouldBe("Not found");
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenException()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new UpdateApplicationByClientId()
        {
            Id = id,
        };

        service.Update(request, Arg.Any<CancellationToken>()).Throws(new Exception("Error"));

        // Act
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
            .GetMethod("PutByIdRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        ((BadRequest<string>)result).Value.ShouldBe("Error");
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new DeleteApplicationByClientId
        {
            Id = id,
        };

        // Act
        var result = await (Task<IResult>)typeof(ApplicationEndpoints)
            .GetMethod("DeleteByIdRoute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [request, service])!;

        // Assert
        result.ShouldBeOfType<NoContent>();
        await service.Received(1).Delete(request, Arg.Any<CancellationToken>());
    }
}
