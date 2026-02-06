// <copyright file="UpdateUserValidatorTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Unit.Tests.Requests.Users.Commands.Update;

using Defra.Identity.Models.Requests.Users.Commands.Update;
using Defra.Identity.Requests.Users.Commands.Validators;
using FluentValidation.TestHelper;
using Xunit;

public class UpdateUserValidatorTests
{
    private readonly UpdateUserValidator validator = new();

    [Fact]
    public void Should_Have_Error_When_DisplayName_Is_Empty()
    {
        var model = new UpdateUser { DisplayName = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact]
    public void Should_Have_Error_When_DisplayName_Exceeds_100_Characters()
    {
        var model = new UpdateUser { DisplayName = new string('a', 101) };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        var model = new UpdateUser { FirstName = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Exceeds_50_Characters()
    {
        var model = new UpdateUser { FirstName = new string('a', 51) };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Empty()
    {
        var model = new UpdateUser { LastName = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Exceeds_50_Characters()
    {
        var model = new UpdateUser { LastName = new string('a', 51) };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var model = new UpdateUser { Email = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var model = new UpdateUser { Email = "invalid-email" };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
  }
