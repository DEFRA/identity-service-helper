// <copyright file="CreateApplicationValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Applications.Commands;

using FluentValidation;

public class CreateApplicationValidator : AbstractValidator<CreateApplication>
{
    public CreateApplicationValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TenantName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Secret).MaximumLength(74)
            .When(x => x.Secret != null);
        RuleFor(x => x.Scopes)
            .Must(x => x?.Sum(y => y?.Length ?? 0) <= 500)
            .When(x => x.Scopes != null)
            .WithMessage("Scopes must not exceed 500 characters in total");
        RuleFor(x => x.RedirectUris)
            .Must(x => x?.Sum(y => y?.Length ?? 0) <= 1000)
            .When(x => x.RedirectUris != null)
            .WithMessage("Redirect URIs must not exceed 1000 characters in total");
    }
}
