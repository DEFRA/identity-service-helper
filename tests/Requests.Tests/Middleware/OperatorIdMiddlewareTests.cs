// <copyright file="OperatorIdMiddlewareTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests.Middleware;

using Defra.Identity.Requests.MetaData;
using Defra.Identity.Requests.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                { "DefraIndentityApiKey", "test-api-key" },
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
        var (middleware, context, next, nextCalled) = CreateContext();
        context.Request.Headers[IdentityHeaderNames.OperatorId] = Guid.NewGuid().ToString();
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
        var (middleware, context, next, nextCalled) = CreateContext();
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
            x => x.ShouldContain($"Header {IdentityHeaderNames.OperatorId} is required."));
    }

    [Fact]
    public async Task InvokeAsync_WhitespaceOperatorIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var (middleware, context, next, nextCalled) = CreateContext();
        context.Request.Headers[IdentityHeaderNames.OperatorId] = "   ";
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
            x => x.ShouldContain($"Header {IdentityHeaderNames.OperatorId} is required."));
    }

    [Fact]
    public async Task InvokeAsync_InvalidGuidOperatorIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var (middleware, context, next, nextCalled) = CreateContext();
        context.Request.Headers[IdentityHeaderNames.OperatorId] = "invalid-guid";
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
            x => x.ShouldContain($"Header {IdentityHeaderNames.OperatorId} must be a valid GUID."));
    }

    [Fact]
    public async Task InvokeAsync_NoMetadata_CallsNext()
    {
        // Arrange
        var (middleware, context, next, nextCalled) = CreateContext();
        // No endpoint setup

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        nextCalled().ShouldBeTrue();
    }

    private static (OperatorIdMiddleware Middleware, HttpContext Context, RequestDelegate Next, Func<bool> NextCalled) CreateContext()
    {
        var middleware = new OperatorIdMiddleware();
        var context = new DefaultHttpContext();
        var nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        return (middleware, context, next, () => nextCalled);
    }

    private static void SetupEndpoint(HttpContext context)
    {
        var metadataItems = new List<object> { new RequiresOperatorId() };
        var metadata = new EndpointMetadataCollection(metadataItems);
        var endpoint = new Endpoint(null, metadata, "Test endpoint");

        context.SetEndpoint(endpoint);
    }
}
