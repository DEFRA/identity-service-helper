// <copyright file="CreateApplicationValidatorTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Tests.Requests.Applications.Commands;

using Defra.Identity.Models.Requests.Applications.Commands;
using FluentValidation.TestHelper;

public class CreateApplicationValidatorTests
{
    private readonly CreateApplicationValidator validator = new();

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        var model = CreateValidRequest();

        model.Id = Guid.Empty;

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var model = CreateValidRequest();

        model.Name = string.Empty;

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_50_Characters()
    {
        var model = CreateValidRequest();

        model.Name = new string('a', 51);

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_TenantName_Is_Empty()
    {
        var model = CreateValidRequest();

        model.TenantName = string.Empty;

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.TenantName);
    }

    [Fact]
    public void Should_Have_Error_When_TenantName_Exceeds_50_Characters()
    {
        var model = CreateValidRequest();

        model.TenantName = new string('a', 51);

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.TenantName);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var model = CreateValidRequest();

        model.Description = string.Empty;

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_100_Characters()
    {
        var model = CreateValidRequest();

        model.Description = new string('a', 101);

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Have_Error_When_Secret_Exceeds_74_Characters()
    {
        var model = CreateValidRequest();

        model.Secret = new string('a', 75);

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Secret);
    }

    [Fact]
    public void Should_Have_Error_When_Concatenated_Scopes_Exceeds_500_Characters()
    {
        var model = CreateValidRequest();

        model.Scopes.Add(new string('a', 250));
        model.Scopes.Add(new string('b', 250));
        model.Scopes.Add(new string('c', 1));

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Scopes);
    }

    [Fact]
    public void Should_Have_Error_When_Concatenated_RedirectUris_Exceeds_1000_Characters()
    {
        var model = CreateValidRequest();

        model.RedirectUris.Add(new string('a', 500));
        model.RedirectUris.Add(new string('b', 500));
        model.RedirectUris.Add(new string('c', 1));

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.RedirectUris);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var model = CreateValidRequest();

        var result = this.validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateApplication CreateValidRequest() =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = "New App",
            TenantName = "New Tenant",
            Description = "New Description",
            Scopes = ["scope1", "scope2"],
            RedirectUris = ["https://localhost/callback"],
            Secret = "secret123",
        };
}
