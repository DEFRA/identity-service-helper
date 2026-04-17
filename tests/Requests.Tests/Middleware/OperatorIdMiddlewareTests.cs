// <copyright file="OperatorIdMiddlewareTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests.Middleware;

using Defra.Identity.Api.Extensions;
using Defra.Identity.Api.Middleware;
using Defra.Identity.Api.Middleware.Headers;
using Defra.Identity.Models.Requests;
using Defra.Identity.Models.Requests.MetaData;
using Defra.Identity.Models.Requests.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class OperatorIdMiddlewareTests
{
    [Fact]
    public void AddRequests_RegistersMiddleware_CanBeResolved()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
            {
                {
                    "DefraIndentityApiKey", "test-api-key"
                },
            })
            .Build();

        // Act
        services.AddLogging();
        services.AddRequests(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var middleware = serviceProvider.GetService<OperatorIdMiddleware>();
        middleware.ShouldNotBeNull();
    }

    [Fact]
    public void UseRequests_WithIMiddleware_DoesNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddRequests(builder.Configuration);
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(() => app.UseRequests());
    }

    [Fact]
    public async Task UseRequests_WithNoEndpoint_ReturnsWithoutProcessing()
    {
        // Arrange
        var logger = Substitute.For<ILogger<OperatorIdMiddleware>>();
        var operatorIdService = Substitute.For<IOperatorIdService>();
        var middleware = new OperatorIdMiddleware(operatorIdService, logger);
        var context = new DefaultHttpContext();
        context.Request.Headers[RequestHeaderNames.ApiKey] = "test-key";

        Task Next(HttpContext ctx)
            => Task.CompletedTask;

        // Act
        await middleware.InvokeAsync(context, Next);

        // Assert
        context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InvokeAsync_WithOperatorIdHeader_CallsNext()
    {
        // Arrange
        var (middleware, context, next, nextCalled, _) = CreateContext();
        context.Request.Headers[RequestHeaderNames.OperatorId] = Guid.NewGuid().ToString();
        SetupEndpoint(context);

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        nextCalled().ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InvokeAsync_MissingOperatorIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var (middleware, context, next, nextCalled, _) = CreateContext();
        context.Response.Body = new MemoryStream();
        SetupEndpoint(context);

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        nextCalled().ShouldBeFalse();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        context.Response.ContentType.ShouldBe("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        responseBody.ShouldSatisfyAllConditions(
            x => x.ShouldContain("missing_header"),
            x => x.ShouldContain($"Header {RequestHeaderNames.OperatorId} is required."));
    }

    [Fact]
    public async Task InvokeAsync_WhitespaceOperatorIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var (middleware, context, next, nextCalled, _) = CreateContext();
        context.Request.Headers[RequestHeaderNames.OperatorId] = "   ";
        context.Response.Body = new MemoryStream();
        SetupEndpoint(context);

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        nextCalled().ShouldBeFalse();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        context.Response.ContentType.ShouldBe("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        responseBody.ShouldSatisfyAllConditions(
            x => x.ShouldContain("missing_header"),
            x => x.ShouldContain($"Header {RequestHeaderNames.OperatorId} is required."));
    }

    [Fact]
    public async Task InvokeAsync_InvalidGuidOperatorIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var (middleware, context, next, nextCalled, _) = CreateContext();
        context.Request.Headers[RequestHeaderNames.OperatorId] = "invalid-guid";
        context.Response.Body = new MemoryStream();
        SetupEndpoint(context);

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        nextCalled().ShouldBeFalse();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        context.Response.ContentType.ShouldBe("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        responseBody.ShouldSatisfyAllConditions(
            x => x.ShouldContain("invalid_header"),
            x => x.ShouldContain($"Header {RequestHeaderNames.OperatorId} must be a valid GUID."));
    }

    [Fact]
    public async Task InvokeAsync_NoMetadata_CallsNext()
    {
        // Arrange
        var (middleware, context, next, nextCalled, _) = CreateContext();

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        nextCalled().ShouldBeTrue();
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_LogsErrorAndReThrows()
    {
        // Arrange
        var (middleware, context, next, _, logger) = CreateContext();
        context.Request.Headers[RequestHeaderNames.OperatorId] = Guid.NewGuid().ToString("D");
        context.Response.Body = new MemoryStream();
        SetupEndpoint(context);
        var exception = new Exception("Test exception");
        next = (_) => throw exception;

        // Act & Assert
        var ex = await Should.ThrowAsync<Exception>(() => middleware.InvokeAsync(context, next));
        ex.ShouldBe(exception);

        logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<Arg.AnyType>(),
            exception,
            Arg.Any<Func<Arg.AnyType, Exception?, string>>());
    }

    private static (
        OperatorIdMiddleware Middleware,
        HttpContext Context,
        RequestDelegate Next,
        Func<bool> NextCalled,
        ILogger<OperatorIdMiddleware> Logger) CreateContext()
    {
        var logger = Substitute.For<ILogger<OperatorIdMiddleware>>();
        var operatorIdService = Substitute.For<IOperatorIdService>();
        var middleware = new OperatorIdMiddleware(operatorIdService, logger);
        var context = new DefaultHttpContext();
        var nextCalled = false;

        return (middleware, context, Next, () => nextCalled, logger);

        Task Next(HttpContext ctx)
        {
            nextCalled = true;
            return Task.CompletedTask;
        }
    }

    private static void SetupEndpoint(HttpContext context)
    {
        var metadataItems = new List<object>
            {
                new RequiresOperatorId(),
            };
        var metadata = new EndpointMetadataCollection(metadataItems);
        var endpoint = new Endpoint(null, metadata, "Test endpoint");

        context.SetEndpoint(endpoint);
    }
}
