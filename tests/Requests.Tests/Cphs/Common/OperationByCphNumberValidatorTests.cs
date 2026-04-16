// <copyright file="OperationByCphNumberValidatorTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests.Cphs.Common;

using Defra.Identity.Models.Requests.Cphs.Common;
using Defra.Identity.Requests.Tests.Cphs.Common.TestData;
using FluentValidation.TestHelper;

public class OperationByCphNumberValidatorTests
{
    private readonly OperationByCphNumberValidator validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_County_Is_Zero()
    {
        // Arrange
        var model = new OperationByCphNumberFake()
        {
            County = 0,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.County);
    }

    [Fact]
    public void Should_Not_Have_Error_When_County_Is_Max_99()
    {
        // Arrange
        var model = new OperationByCphNumberFake()
        {
            County = 99,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.County);
    }

    [Fact]
    public void Should_Have_Error_When_County_Negative()
    {
        // Arrange
        var model = new OperationByCphNumberFake()
        {
            County = -1,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.County);
    }

    [Fact]
    public void Should_Have_Error_When_County_Is_Above_Max_99()
    {
        // Arrange
        var model = new OperationByCphNumberFake()
        {
            County = 100,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.County);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Parish_Is_Zero()
    {
        // Arrange
        var model = new OperationByCphNumberFake()
        {
            Parish = 0,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Parish);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Parish_Is_Max_999()
    {
        // Arrange
        var model = new OperationByCphNumberFake()
        {
            Parish = 999,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Parish);
    }

    [Fact]
    public void Should_Have_Error_When_Parish_Negative()
    {
        // Arrange
        var model = new OperationByCphNumberFake()
        {
            Parish = -1,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Parish);
    }

    [Fact]
    public void Should_Have_Error_When_Parish_Is_Above_Max_999()
    {
        // Arrange
        var model = new OperationByCphNumberFake()
        {
            Parish = 1000,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Parish);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Holding_Is_Zero()
    {
        // Arrange
        var model = new OperationByCphNumberFake()
        {
            Holding = 0,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Holding);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Holding_Is_Max_9999()
    {
        // Arrange
        var model = new OperationByCphNumberFake()
        {
            Holding = 9999,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Holding);
    }

    [Fact]
    public void Should_Have_Error_When_Holding_Negative()
    {
        // Arrange
        var model = new OperationByCphNumberFake()
        {
            Holding = -1,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Holding);
    }

    [Fact]
    public void Should_Have_Error_When_Holding_Is_Above_Max_9999()
    {
        // Arrange
        var model = new OperationByCphNumberFake()
        {
            Holding = 10000,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Holding);
    }
}
