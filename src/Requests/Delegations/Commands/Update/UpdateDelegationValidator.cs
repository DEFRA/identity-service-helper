// <copyright file="UpdateDelegationValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Commands.Update;

using FluentValidation;

public class UpdateDelegationValidator : AbstractValidator<UpdateDelegation>
{
    public UpdateDelegationValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
