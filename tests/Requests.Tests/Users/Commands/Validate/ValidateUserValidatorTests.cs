// <copyright file="ValidateUserValidatorTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Tests.Users.Commands.Validate;

using Defra.Identity.Requests.Users.Commands.Validate;
using FluentValidation.TestHelper;
using Xunit;

public class ValidateUserValidatorTests
{
    private readonly ValidateUserValidator validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var model = new ValidateUser { Email = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var model = new ValidateUser { Email = "invalid-email" };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Email_Is_Valid()
    {
        var model = new ValidateUser { Email = "test@example.com" };
        var result = this.validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
