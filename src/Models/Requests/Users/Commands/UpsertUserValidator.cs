// <copyright file="UpsertUserValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Commands;

using FluentValidation;

public class UpsertUserValidator : AbstractValidator<UpsertUserById>
{
    public UpsertUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .When(x => x.Id.HasValue);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(256).EmailAddress();
    }
}
