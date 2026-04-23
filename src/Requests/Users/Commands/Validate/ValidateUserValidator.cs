// <copyright file="ValidateUserValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Users.Commands.Validate;

using Defra.Identity.Requests.Users.Commands.Update;
using FluentValidation;

public class ValidateUserValidator : AbstractValidator<ValidateUser>
{
    public ValidateUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
