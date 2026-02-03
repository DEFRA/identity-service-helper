// <copyright file="ValidationFilterTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Unit.Tests.Requests.Filters;

using Defra.Identity.Requests.Filters;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Shouldly;
using Xunit;

public class ValidationFilterTests
{
    private readonly IValidator<TestModel> _validator = Substitute.For<IValidator<TestModel>>();

    public class TestModel
    {
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public async Task InvokeAsync_Returns_BadRequest_When_Argument_Missing()
    {
        // Arrange
        var filter = new ValidationFilter<TestModel>(this._validator);
        var context = Substitute.For<EndpointFilterInvocationContext>();
        context.Arguments.Returns(new List<object?>());

        // Act
        var result = await filter.InvokeAsync(context, _ => ValueTask.FromResult<object?>(null));

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
    }

    [Fact]
    public async Task InvokeAsync_Returns_ValidationProblem_When_Validation_Fails()
    {
        // Arrange
        var filter = new ValidationFilter<TestModel>(this._validator);
        var model = new TestModel();
        var context = Substitute.For<EndpointFilterInvocationContext>();
        context.Arguments.Returns(new List<object?> { model });

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required"),
        };
        this._validator.ValidateAsync(model, default)
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await filter.InvokeAsync(context, _ => ValueTask.FromResult<object?>(null));

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
    }

    [Fact]
    public async Task InvokeAsync_Calls_Next_When_Validation_Succeeds()
    {
        // Arrange
        var filter = new ValidationFilter<TestModel>(this._validator);
        var model = new TestModel { Name = "Valid" };
        var context = Substitute.For<EndpointFilterInvocationContext>();
        context.Arguments.Returns(new List<object?> { model });

        this._validator.ValidateAsync(model, default)
            .Returns(new ValidationResult());

        var nextCalled = false;
        EndpointFilterDelegate next = _ =>
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>(Results.Ok());
        };

        // Act
        var result = await filter.InvokeAsync(context, next);

        // Assert
        nextCalled.ShouldBeTrue();
        result.ShouldBeOfType<Ok>();
    }
}
