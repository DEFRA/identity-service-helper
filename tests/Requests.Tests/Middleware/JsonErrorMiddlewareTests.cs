// <copyright file="JsonErrorMiddlewareTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests.Middleware;

using System.Text.Json;
using Defra.Identity.Requests.Middleware;
using Microsoft.AspNetCore.Http;

public class JsonErrorMiddlewareTests
{
    private class TestJsonErrorMiddleware : JsonErrorMiddleware
    {
        public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await WriteJsonErrorAsync(context, StatusCodes.Status400BadRequest, "TestCode", "TestMessage", new { Detail = "TestDetail" });
        }
    }

    [Fact]
    public async Task WriteJsonErrorAsync_Writes_Correct_Response()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.TraceIdentifier = "test-trace-id";
        context.Request.Path = "/test-path";

        var middleware = new TestJsonErrorMiddleware();

        // Act
        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        // Assert
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        context.Response.ContentType.ShouldBe("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        var json = JsonDocument.Parse(responseBody);
        var error = json.RootElement.GetProperty("error");

        error.GetProperty("code").GetString().ShouldBe("TestCode");
        error.GetProperty("message").GetString().ShouldBe("TestMessage");
        error.GetProperty("traceId").GetString().ShouldBe("test-trace-id");
        error.GetProperty("path").GetString().ShouldBe("/test-path");
        error.GetProperty("details").GetProperty("Detail").GetString().ShouldBe("TestDetail");
    }
}
