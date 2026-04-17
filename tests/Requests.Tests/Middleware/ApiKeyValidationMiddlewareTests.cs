// <copyright file="ApiKeyValidationMiddlewareTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests.Middleware;

using Defra.Identity.Api.Extensions;
using Defra.Identity.Api.Middleware;
using Defra.Identity.Api.Middleware.Headers;
using Defra.Identity.Models.Requests;
using Defra.Identity.Models.Requests.MetaData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class ApiKeyValidationMiddlewareTests
{
    [Fact]
    public void AddRequests_RegistersMiddleware_CanBeResolved()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "DefraIndentityApiKey", "test-api-key" },
            })
            .Build();

        // Act
        services.AddLogging();
        services.AddRequests(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var middleware = serviceProvider.GetService<ApiKeyValidationMiddleware>();
        middleware.ShouldNotBeNull();
    }

    [Fact]
    public void UseRequests_WithIMiddleware_DoesNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddLogging();
        builder.Services.AddRequests(builder.Configuration);
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(() => app.UseRequests());
    }

    [Fact]
    public async Task UseRequests_WithNoEndpoint_ReturnsWithoutProcessing()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ApiKeyValidationMiddleware>>();
        var middleware = new ApiKeyValidationMiddleware("test-key", logger);
        var context = new DefaultHttpContext();
        context.Request.Headers[RequestHeaderNames.ApiKey] = "test-key";
        RequestDelegate next = (ctx) => Task.CompletedTask;

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task UseRequests_WithIgnoreKey_ReturnsWithoutProcessing()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ApiKeyValidationMiddleware>>();
        var middleware = new ApiKeyValidationMiddleware("test-key", logger);
        var endpoint = Substitute.For<IEndpointFeature>();
        endpoint.Endpoint = new Endpoint(null, new EndpointMetadataCollection([new IgnoreApiKeyCheck()]), "fake endpoint");
        var context = new DefaultHttpContext();
        context.Request.Headers[RequestHeaderNames.ApiKey] = "test-key";
        context.Features.Set(endpoint);

        RequestDelegate next = (ctx) => Task.CompletedTask;

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
    }

    [Theory]
    [InlineData("")]
    [InlineData("No-valid-key")]
    public async Task UseRequests_WithMissingKey_ReturnsErrorJson(string keyValue)
    {
        // Arrange
        var logger = Substitute.For<ILogger<ApiKeyValidationMiddleware>>();
        var middleware = new ApiKeyValidationMiddleware("test-key", logger);
        var endpoint = Substitute.For<IEndpointFeature>();
        endpoint.Endpoint = new Endpoint(null, null, "fake endpoint");
        var context = new DefaultHttpContext();
        context.Request.Headers[RequestHeaderNames.ApiKey] = keyValue;
        context.Features.Set(endpoint);

        RequestDelegate next = (ctx) => Task.CompletedTask;

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_LogsErrorAndReThrows()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ApiKeyValidationMiddleware>>();
        var middleware = new ApiKeyValidationMiddleware("test-key", logger);

        var endpoint = Substitute.For<IEndpointFeature>();
        endpoint.Endpoint = new Endpoint(null, new EndpointMetadataCollection([new RequiresOperatorId()]), "fake endpoint");

        var context = new DefaultHttpContext();
        context.Request.Headers[RequestHeaderNames.ApiKey] = "test-key";
        context.Features.Set(endpoint);

        var exception = new Exception("Test exception");
        RequestDelegate next = (ctx) => throw exception;

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
}
