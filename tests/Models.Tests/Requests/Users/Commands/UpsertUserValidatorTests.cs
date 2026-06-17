// <copyright file="UpsertUserValidatorTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Tests.Requests.Users.Commands;

using Defra.Identity.Models.Requests.Users.Commands;
using FluentValidation.TestHelper;

public class UpsertUserValidatorTests
{
    private readonly UpsertUserValidator validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_Null()
    {
        var model = new UpsertUserById { Id = null };
        var result = this.validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        var model = new UpsertUserById { Id = Guid.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Have_Error_When_DisplayName_Is_Empty()
    {
        var model = new UpsertUserById { DisplayName = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact]
    public void Should_Have_Error_When_DisplayName_Exceeds_100_Characters()
    {
        var model = new UpsertUserById { DisplayName = new string('a', 101) };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        var model = new UpsertUserById { FirstName = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Exceeds_50_Characters()
    {
        var model = new UpsertUserById { FirstName = new string('a', 51) };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Empty()
    {
        var model = new UpsertUserById { LastName = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Exceeds_50_Characters()
    {
        var model = new UpsertUserById { LastName = new string('a', 51) };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var model = new UpsertUserById { Email = string.Empty };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_EmailAddress_Is_WhiteSpace()
    {
        var model = new UpsertUserById { Email = "    ", };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Exceeds_256_Characters()
    {
        var model = new UpsertUserById { Email = new string('a', 257) };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var model = new UpsertUserById { Email = "invalid-email" };
        var result = this.validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid_Without_Id()
    {
        var model = new UpsertUserById
        {
            DisplayName = "John Doe", FirstName = "John", LastName = "Doe", Email = "john.doe@example.com",
        };

        var result = this.validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid_With_Id()
    {
        var model = new UpsertUserById
        {
            Id = Guid.NewGuid(),
            DisplayName = "John Doe",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
        };

        var result = this.validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
