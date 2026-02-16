// <copyright file="CreateApplicationValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Applications.Commands.Create;

using FluentValidation;

public class CreateApplicationValidator : AbstractValidator<CreateApplication>
{
    public CreateApplicationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TenantName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ClientId).NotEmpty();
    }
}
