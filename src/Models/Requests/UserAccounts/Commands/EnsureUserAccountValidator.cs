// <copyright file="EnsureUserAccountValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.UserAccounts.Commands;

using FluentValidation;

public class EnsureUserAccountValidator : AbstractValidator<EnsureUserAccount>
{
    public EnsureUserAccountValidator()
    {
        RuleFor(x => x.Sub).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(256).EmailAddress();
        RuleFor(x => x.GivenName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FamilyName).NotEmpty().MaximumLength(50);
    }
}
