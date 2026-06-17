// <copyright file="CreateCphDelegationValidatorTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Tests.Requests.Delegations.Commands;

using Defra.Identity.Models.Requests.Delegations.Commands;
using FluentValidation.TestHelper;

public class CreateCphDelegationValidatorTests
{
    private readonly CreateCphDelegationValidator validator = new();

    [Fact]
    public void Should_Have_Error_When_CountyParishHoldingId_Is_Empty()
    {
        var model = new CreateCphDelegation()
        {
            CountyParishHoldingId = Guid.Empty,
            DelegatedUserRoleId = Guid.NewGuid(),
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserEmail = "test@test.com",
        };

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CountyParishHoldingId);
    }

    [Fact]
    public void Should_Have_Error_When_DelegatingUserId_Is_Empty()
    {
        var model = new CreateCphDelegation()
        {
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatedUserRoleId = Guid.NewGuid(),
            DelegatingUserId = Guid.Empty,
            DelegatedUserEmail = "test@test.com",
        };

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DelegatingUserId);
    }

    [Fact]
    public void Should_Have_Error_When_DelegatingUserRoleId_Is_Empty()
    {
        var model = new CreateCphDelegation()
        {
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatedUserRoleId = Guid.Empty,
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserEmail = "test@test.com",
        };

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DelegatedUserRoleId);
    }

    [Fact]
    public void Should_Have_Error_When_DelegatedUserEmail_Is_Empty()
    {
        var model = new CreateCphDelegation()
        {
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatedUserRoleId = Guid.NewGuid(),
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserEmail = string.Empty,
        };

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DelegatedUserEmail);
    }

    [Fact]
    public void Should_Have_Error_When_DelegatedUserEmail_Is_WhiteSpace()
    {
        var model = new CreateCphDelegation()
        {
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatedUserRoleId = Guid.NewGuid(),
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserEmail = "    ",
        };

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DelegatedUserEmail);
    }

    [Fact]
    public void Should_Have_Error_When_DelegatedUserEmail_Exceeds_256_Characters()
    {
        var model = new CreateCphDelegation()
        {
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatedUserRoleId = Guid.NewGuid(),
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserEmail = new string('a', 257),
        };

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DelegatedUserEmail);
    }

    [Fact]
    public void Should_Have_Error_When_DelegatedUserEmail_Is_Invalid()
    {
        var model = new CreateCphDelegation()
        {
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatedUserRoleId = Guid.NewGuid(),
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserEmail = "invalid-email",
        };

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.DelegatedUserEmail);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var model = new CreateCphDelegation()
        {
            CountyParishHoldingId = Guid.NewGuid(),
            DelegatedUserRoleId = Guid.NewGuid(),
            DelegatingUserId = Guid.NewGuid(),
            DelegatedUserEmail = "test@test.com",
        };

        var result = this.validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
