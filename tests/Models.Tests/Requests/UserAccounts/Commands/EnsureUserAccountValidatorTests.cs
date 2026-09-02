// <copyright file="EnsureUserAccountValidatorTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Tests.Requests.UserAccounts.Commands;

using Defra.Identity.Models.Requests.UserAccounts.Commands;
using FluentValidation.TestHelper;

public class EnsureUserAccountValidatorTests
{
    private readonly EnsureUserAccountValidator validator = new();

    [Fact]
    public void Should_Not_Have_Errors_For_Complete_Claims()
    {
        var result = this.validator.TestValidate(Valid());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_Sub_Is_Empty()
    {
        var model = new EnsureUserAccount
        {
            Sub = string.Empty,
            Email = "jane.farmer@example.com",
            GivenName = "Jane",
            FamilyName = "Farmer",
        };

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Sub);
    }

    [Fact]
    public void Should_Have_Error_When_Sub_Exceeds_The_Maximum_Length()
    {
        var model = new EnsureUserAccount
        {
            Sub = new string('a', 257),
            Email = "jane.farmer@example.com",
            GivenName = "Jane",
            FamilyName = "Farmer",
        };

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Sub);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Not_An_Email_Address()
    {
        var model = new EnsureUserAccount
        {
            Sub = "subject",
            Email = "not-an-email",
            GivenName = "Jane",
            FamilyName = "Farmer",
        };

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Given_Name_Is_Blank()
    {
        var model = new EnsureUserAccount
        {
            Sub = "subject",
            Email = "jane.farmer@example.com",
            GivenName = " ",
            FamilyName = "Farmer",
        };

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.GivenName);
    }

    [Fact]
    public void Should_Have_Error_When_Family_Name_Is_Blank()
    {
        var model = new EnsureUserAccount
        {
            Sub = "subject",
            Email = "jane.farmer@example.com",
            GivenName = "Jane",
            FamilyName = string.Empty,
        };

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.FamilyName);
    }

    private static EnsureUserAccount Valid()
    {
        return new EnsureUserAccount
        {
            Sub = "9f3a1c2e-0b6d-4f4e-9d2a-7c8b1e5f0a3d",
            Email = "jane.farmer@example.com",
            GivenName = "Jane",
            FamilyName = "Farmer",
        };
    }
}
