// <copyright file="ValidateUserValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Commands;

using FluentValidation;

public class ValidateUserValidator : AbstractValidator<ValidateUser>
{
    public ValidateUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
