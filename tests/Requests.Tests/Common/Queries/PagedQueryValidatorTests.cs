// <copyright file="PagedQueryValidatorTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests.Common.Queries;

using Defra.Identity.Requests.Common.Queries;
using FluentValidation.TestHelper;

public class PagedQueryValidatorTests
{
    private readonly PagedQueryValidator validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_PageNumber_Is_One()
    {
        // Arrange
        var model = new PagedQuery()
        {
            PageNumber = 1,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Should_Not_Have_Error_When_PageNumber_Is_Max()
    {
        // Arrange
        var model = new PagedQuery()
        {
            PageNumber = int.MaxValue,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Should_Have_Error_When_PageNumber_Is_Zero()
    {
        // Arrange
        var model = new PagedQuery()
        {
            PageNumber = 0,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Should_Have_Error_When_PageNumber_Is_Negative()
    {
        // Arrange
        var model = new PagedQuery()
        {
            PageNumber = -1,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Should_Not_Have_Error_When_PageSize_Is_One()
    {
        // Arrange
        var model = new PagedQuery()
        {
            PageSize = 1,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Should_Not_Have_Error_When_PageSize_Is_Max_500()
    {
        // Arrange
        var model = new PagedQuery()
        {
            PageSize = 500,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Should_Have_Error_When_PageSize_Is_Zero()
    {
        // Arrange
        var model = new PagedQuery()
        {
            PageSize = 0,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Should_Have_Error_When_PageSize_Is_Negative()
    {
        // Arrange
        var model = new PagedQuery()
        {
            PageSize = -1,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Should_Have_Error_When_PageSize_Is_Above_Max_500()
    {
        // Arrange
        var model = new PagedQuery()
        {
            PageSize = 501,
        };

        // Act
        var result = this.validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
