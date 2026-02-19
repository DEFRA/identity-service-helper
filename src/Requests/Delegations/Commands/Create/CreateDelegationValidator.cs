// <copyright file="CreateDelegationValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Commands.Create;

using FluentValidation;

public class CreateDelegationValidator : AbstractValidator<CreateDelegation>
{
    public CreateDelegationValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
