// <copyright file="OperationByStringIdMappingFilterTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Tests.Filters;

using Defra.Identity.Api.Filters;
using Defra.Identity.Models.Requests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using NSubstitute;

public class OperationByStringIdMappingFilterTests
{
    private readonly OperationByStringIdMappingFilter<TestModel> filter = new();

    [Fact]
    public async Task InvokeAsync_Returns_BadRequest_When_Argument_Missing()
    {
        // Arrange
        var context = Substitute.For<EndpointFilterInvocationContext>();
        context.Arguments.Returns([]);

        // Act
        var result = await this.filter
            .InvokeAsync(context, _ => ValueTask.FromResult<object?>(null));

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
    }

    [Fact]
    public async Task InvokeAsync_Returns_InvalidOperationException_When_Id_Mismatch()
    {
        // Arrange
        var model = new TestModel { Id = "TestId", };
        var context = Substitute.For<EndpointFilterInvocationContext>();
        var routeValue = Substitute.For<IRouteValuesFeature>();
        routeValue.RouteValues = new RouteValueDictionary { { "id2", "TestId2" }, };
        var httpContext = new DefaultHttpContext();
        httpContext.Features.Set(routeValue);
        context.Arguments.Returns([model]);
        context.HttpContext.Returns(httpContext);

        // Act
        try
        {
            await this.filter.InvokeAsync(context, _ => ValueTask.FromResult<object?>(null));
        }
        catch (Exception e)
        {
            // Assert
            e.ShouldBeOfType<InvalidOperationException>();
        }
    }

    [Fact]
    public async Task InvokeAsync_Calls_Next_When_Validation_Succeeds()
    {
        // Arrange
        var model = new TestModel { Id = "TestId", };
        var context = Substitute.For<EndpointFilterInvocationContext>();
        var routeValue = Substitute.For<IRouteValuesFeature>();
        routeValue.RouteValues = new RouteValueDictionary { { "id", "TestId" }, };
        var httpContext = new DefaultHttpContext();
        httpContext.Features.Set(routeValue);
        context.Arguments.Returns([model]);
        context.HttpContext.Returns(httpContext);

        var nextCalled = false;

        // Act
        var result = await this.filter.InvokeAsync(context, Next);

        // Assert
        nextCalled.ShouldBeTrue();
        result.ShouldBeOfType<Ok>();

        return;

#pragma warning disable SA1313
        ValueTask<object?> Next(EndpointFilterInvocationContext _)
#pragma warning restore SA1313
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>(Results.Ok());
        }
    }

    private class TestModel : OperationById<string>;
}
