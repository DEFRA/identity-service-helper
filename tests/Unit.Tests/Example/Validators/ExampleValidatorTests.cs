// <copyright file="ExampleValidatorTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Unit.Tests.Example.Validators;

using Defra.Identity.Api.Example.Models;
using Defra.Identity.Api.Example.Validators;
using FluentValidation.TestHelper;
using MongoDB.Bson;

public class ExampleValidatorTests
{
    private readonly ExampleValidator validator = new();

    [Fact]
    public void ValidModel()
    {
        var model = new ExampleModel
        {
            Id = default,
            Value = "some value",
            Name = "Test",
            Counter = 0,
        };
        var result = validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InvalidName()
    {
        var model = new ExampleModel
        {
            Id = default,
            Value = "Some value",
            Name = "Test $FOO someName", // letters/numbers/spaces only
        };
        var result = validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(b => b.Name);
    }

    [Fact]
    public void InvalidCounter()
    {
        var model = new ExampleModel
        {
            Id = default,
            Value = "Some value",
            Name = "Test",
            Counter = -1,
        };
        var result = validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(b => b.Counter);
    }

    [Fact]
    public void EmptyValue()
    {
        var model = new ExampleModel
        {
            Id = default,
            Value = string.Empty,
            Name = "Test",
            Counter = 0,
        };
        var result = validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(b => b.Value);
    }
}
