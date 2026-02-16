// <copyright file="UpdateApplicationValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Applications.Commands.Update;

using FluentValidation;

public class UpdateApplicationValidator : AbstractValidator<UpdateApplication>
{
    public UpdateApplicationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TenantName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ClientId).NotEmpty();
    }
}
