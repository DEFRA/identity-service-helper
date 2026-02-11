// <copyright file="ValidationFilterTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests.Filters;

using Defra.Identity.Requests.Filters;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

public class ValidationFilterTests
{
    private readonly IValidator<TestModel> validator = Substitute.For<IValidator<TestModel>>();
    private readonly ValidationFilter<TestModel> filter;

    public ValidationFilterTests()
    {
        this.filter = new ValidationFilter<TestModel>(this.validator);
    }

    [Fact]
    public async Task InvokeAsync_Returns_BadRequest_When_Argument_Missing()
    {
        // Arrange
        var context = Substitute.For<EndpointFilterInvocationContext>();
        context.Arguments.Returns([]);

        // Act
        var result = await this.filter.InvokeAsync(context, _ => ValueTask.FromResult<object?>(null));

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
    }

    [Fact]
    public async Task InvokeAsync_Returns_UnprocessableEntity_When_Validation_Fails()
    {
        // Arrange
        var model = new TestModel();
        var context = Substitute.For<EndpointFilterInvocationContext>();
        context.Arguments.Returns([model]);

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required"),
        };

        this.validator.ValidateAsync(model, Arg.Any<CancellationToken>()).Returns(new ValidationResult(validationFailures));

        // Act
        var result = await this.filter.InvokeAsync(context, _ => ValueTask.FromResult<object?>(null));

        // Assert
        result.ShouldBeOfType<UnprocessableEntity<IDictionary<string, string[]>>>();
    }

    [Fact]
    public async Task InvokeAsync_Calls_Next_When_Validation_Succeeds()
    {
        // Arrange
        var model = new TestModel { Name = "Valid" };
        var context = Substitute.For<EndpointFilterInvocationContext>();
        context.Arguments.Returns([model]);

        this.validator.ValidateAsync(model, Arg.Any<CancellationToken>()).Returns(new ValidationResult());

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

    public class TestModel
    {
        public string Name { get; set; } = string.Empty;
    }
}
