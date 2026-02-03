// <copyright file="CreateUserValidatorTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Unit.Tests.Requests.Users.Commands.Create;

using Defra.Identity.Requests.Users.Commands.Create;
using FluentValidation.TestHelper;
using Xunit;

public class CreateUserValidatorTests
{
    private readonly CreateUserValidator validator = new();

    [Fact]
    public void Should_Have_Error_When_DisplayName_Is_Empty()
    {
        var model = new CreateUser { DisplayName = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact]
    public void Should_Have_Error_When_DisplayName_Exceeds_100_Characters()
    {
        var model = new CreateUser { DisplayName = new string('a', 101) };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        var model = new CreateUser { FirstName = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Exceeds_50_Characters()
    {
        var model = new CreateUser { FirstName = new string('a', 51) };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Empty()
    {
        var model = new CreateUser { LastName = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Exceeds_50_Characters()
    {
        var model = new CreateUser { LastName = new string('a', 51) };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_Have_Error_When_EmailAddress_Is_Empty()
    {
        var model = new CreateUser { Email = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_EmailAddress_Is_Invalid()
    {
        var model = new CreateUser { Email = "invalid-email" };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_OperatorId_Is_Empty()
    {
        var model = new CreateUser { OperatorId = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.OperatorId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var model = new CreateUser
        {
            DisplayName = "John Doe",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            OperatorId = Guid.NewGuid().ToString(),
        };
        var result = this.validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
