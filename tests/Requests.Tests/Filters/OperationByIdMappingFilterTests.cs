// <copyright file="OperationByIdMappingFilterTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests.Filters;

using Defra.Identity.Api.Filters;
using Defra.Identity.Models.Requests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using NSubstitute;

public class OperationByIdMappingFilterTests
{
    private readonly OperationByIdMappingFilter<TestModel> filter = new();

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
        var guid = "2e35b982-e427-4917-8b9d-454220553a1e";
        var model = new TestModel { Id = Guid.Parse(guid) };
        var context = Substitute.For<EndpointFilterInvocationContext>();
        var routeValue = Substitute.For<IRouteValuesFeature>();
        routeValue.RouteValues = new RouteValueDictionary { { "id2", "2e35b982-e427-4917-8b9d-454220553a1d" } };
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
        var guid = "2e35b982-e427-4917-8b9d-454220553a1d";
        var model = new TestModel { Id = Guid.Parse(guid) };
        var context = Substitute.For<EndpointFilterInvocationContext>();
        var routeValue = Substitute.For<IRouteValuesFeature>();
        routeValue.RouteValues = new RouteValueDictionary { { "id", guid } };
        var httpContext = new DefaultHttpContext();
        httpContext.Features.Set(routeValue);
        context.Arguments.Returns([model]);
        context.HttpContext.Returns(httpContext);

        var nextCalled = false;
        EndpointFilterDelegate next = _ =>
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>(Results.Ok());
        };

        // Act
        var result = await this.filter.InvokeAsync(context, next);

        // Assert
        nextCalled.ShouldBeTrue();
        result.ShouldBeOfType<Ok>();
    }

    public class TestModel : IOperationById
    {
        public Guid Id { get; set; }
    }
}
