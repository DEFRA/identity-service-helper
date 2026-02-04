// <copyright file="CorrellationIdMiddlewareTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Unit.Tests.Requests.Middleware;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Defra.Identity.Requests;
using Defra.Identity.Requests.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

public class CorrellationIdMiddlewareTests
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
        var middleware = serviceProvider.GetService<CorrellationIdMiddleware>();
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
    public async Task InvokeAsync_WithCorrelationIdHeader_CallsNext()
    {
        // Arrange
        var middleware = new CorrellationIdMiddleware();
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = "test-correlation-id";

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
    public async Task InvokeAsync_MissingCorrelationIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var middleware = new CorrellationIdMiddleware();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

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
        responseBody.ShouldContain($"Header {IdentityHeaderNames.CorrelationId} is required.");
    }

    [Fact]
    public async Task InvokeAsync_WhitespaceCorrelationIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var middleware = new CorrellationIdMiddleware();
        var context = new DefaultHttpContext();
        context.Request.Headers[IdentityHeaderNames.CorrelationId] = "   ";
        context.Response.Body = new MemoryStream();

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
        responseBody.ShouldContain($"Header {IdentityHeaderNames.CorrelationId} is required.");
    }
}
