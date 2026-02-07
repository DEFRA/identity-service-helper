// <copyright file="OperatorIdMiddlewareTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests.Middleware;

using Defra.Identity.Requests.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

public class OperatorIdMiddlewareTests
{
    [Fact]
    public void AddRequests_RegistersMiddleware_CanBeResolved()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "DefraIndentityApiKey", "test-api-key" }
            })
            .Build();

        // Act
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
    public async Task InvokeAsync_WithOperatorIdHeader_CallsNext()
    {
        // Arrange
        var middleware = new OperatorIdMiddleware();
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.OperatorId] = Guid.NewGuid().ToString();
        SetupEndpoint(context, useCommandRequestHeaders: true);

        var nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        nextCalled.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InvokeAsync_MissingOperatorIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var middleware = new OperatorIdMiddleware();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        SetupEndpoint(context, useCommandRequestHeaders: true);

        var nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        nextCalled.ShouldBeFalse();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        context.Response.ContentType.ShouldBe("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        responseBody.ShouldContain("missing_header");
        responseBody.ShouldContain($"Header {IdentityHeaderNames.OperatorId} is required.");
    }

    [Fact]
    public async Task InvokeAsync_WhitespaceOperatorIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var middleware = new OperatorIdMiddleware();
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.OperatorId] = "   ";
        context.Response.Body = new MemoryStream();
        SetupEndpoint(context, useCommandRequestHeaders: true);

        var nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        nextCalled.ShouldBeFalse();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        context.Response.ContentType.ShouldBe("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        responseBody.ShouldContain("missing_header");
        responseBody.ShouldContain($"Header {IdentityHeaderNames.OperatorId} is required.");
    }

    [Fact]
    public async Task InvokeAsync_InvalidGuidOperatorIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var middleware = new OperatorIdMiddleware();
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.OperatorId] = "invalid-guid";
        context.Response.Body = new MemoryStream();
        SetupEndpoint(context, useCommandRequestHeaders: true);

        var nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        nextCalled.ShouldBeFalse();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        context.Response.ContentType.ShouldBe("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        responseBody.ShouldContain("invalid_header");
        responseBody.ShouldContain($"Header {IdentityHeaderNames.OperatorId} must be a valid GUID.");
    }

    [Fact]
    public async Task InvokeAsync_DoesNotUseCommandRequestHeaders_CallsNext()
    {
        // Arrange
        var middleware = new OperatorIdMiddleware();
        var context = new DefaultHttpContext();
        SetupEndpoint(context, useCommandRequestHeaders: false);

        var nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        nextCalled.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
    }

    private static void SetupEndpoint(HttpContext context, bool useCommandRequestHeaders)
    {
        var actionDescriptor = new ControllerActionDescriptor
        {
            Parameters = new List<ParameterDescriptor>()
        };

        if (useCommandRequestHeaders)
        {
            actionDescriptor.Parameters.Add(new ParameterDescriptor
            {
                ParameterType = typeof(CommandRequestHeaders)
            });
        }

        var metadata = new EndpointMetadataCollection(actionDescriptor);
        var endpoint = new Endpoint(null, metadata, "Test endpoint");

        context.SetEndpoint(endpoint);
    }
}
