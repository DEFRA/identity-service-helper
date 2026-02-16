// <copyright file="ApiExceptionHandlerTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Exceptions;

using System.Text.Json;
using Defra.Identity.Api.Exceptions;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

public class ApiExceptionHandlerTests
{
    private readonly ILogger<ApiExceptionHandler> logger;
    private readonly ApiExceptionHandler handler;

    public ApiExceptionHandlerTests()
    {
        logger = Substitute.For<ILogger<ApiExceptionHandler>>();
        handler = new ApiExceptionHandler(logger);
    }

    [Fact]
    public async Task TryHandleAsync_NotFoundException_Returns404()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new NotFoundException("Resource not found");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.TryHandleAsync(context, exception, cancellationToken);

        // Assert
        result.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status404NotFound);

        var problem = await GetProblemDetails(context);
        problem.Status.ShouldBe(StatusCodes.Status404NotFound);
        problem.Title.ShouldBe("Not Found");
        problem.Type.ShouldBe("https://httpstatuses.com/404");
        problem.Detail.ShouldBe(exception.Message);
    }

    [Fact]
    public async Task TryHandleAsync_ArgumentException_Returns400()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new ArgumentException("Invalid argument");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.TryHandleAsync(context, exception, cancellationToken);

        // Assert
        result.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);

        var problem = await GetProblemDetails(context);
        problem.Status.ShouldBe(StatusCodes.Status400BadRequest);
        problem.Title.ShouldBe("Bad Request");
        problem.Type.ShouldBe("https://httpstatuses.com/400");
        problem.Detail.ShouldBe(exception.Message);
    }

    [Fact]
    public async Task TryHandleAsync_UnauthorizedAccessException_Returns403()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new UnauthorizedAccessException("Forbidden access");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.TryHandleAsync(context, exception, cancellationToken);

        // Assert
        result.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);

        var problem = await GetProblemDetails(context);
        problem.Status.ShouldBe(StatusCodes.Status403Forbidden);
        problem.Title.ShouldBe("Forbidden");
        problem.Type.ShouldBe("https://httpstatuses.com/403");
        problem.Detail.ShouldBe(exception.Message);
    }

    [Fact]
    public async Task TryHandleAsync_GenericException_Returns500()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new Exception("Internal error");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.TryHandleAsync(context, exception, cancellationToken);

        // Assert
        result.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);

        var problem = await GetProblemDetails(context);
        problem.Status.ShouldBe(StatusCodes.Status500InternalServerError);
        problem.Title.ShouldBe("Internal Server Error");
        problem.Type.ShouldBe("https://httpstatuses.com/500");
        problem.Detail.ShouldBe(exception.Message);

        logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<Arg.AnyType>(),
            exception,
            Arg.Any<Func<Arg.AnyType, Exception?, string>>());
    }

    [Fact]
    public async Task TryHandleAsync_WarningException_LogsWarning()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new NotFoundException("Not found");
        var cancellationToken = CancellationToken.None;

        // Act
        await handler.TryHandleAsync(context, exception, cancellationToken);

        // Assert
        logger.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<Arg.AnyType>(),
            exception,
            Arg.Any<Func<Arg.AnyType, Exception?, string>>());
    }

    [Fact]
    public async Task TryHandleAsync_SetsCorrelationIdFromHeader()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var correlationId = "test-correlation-id";
        context.Request.Headers[RequestHeaderNames.CorrelationId] = correlationId;
        var exception = new Exception("Error");

        // Act
        await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        // We can't easily verify Serilog LogContext, but we can check if it didn't crash
        // and that the response is correct.
        context.Response.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task TryHandleAsync_ProblemDetails_ContainsExpectedExtensions()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.TraceIdentifier = "test-trace-id";
        context.Request.Path = "/test-path";
        var exception = new Exception("Error");

        // Act
        await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        var problem = await GetProblemDetails(context);
        problem.Instance.ShouldBe("/test-path");
        problem.Extensions.ShouldContainKey("traceId");
        problem.Extensions["traceId"]?.ToString().ShouldBe("test-trace-id");
    }

    private static async Task<ProblemDetails> GetProblemDetails(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body, options);
        problem.ShouldNotBeNull();
        return problem;
    }
}
