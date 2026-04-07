// <copyright file="CreateCphDelegationValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Commands.Create;

using FluentValidation;

public class CreateCphDelegationValidator : AbstractValidator<CreateCphDelegation>
{
    public CreateCphDelegationValidator()
    {
        RuleFor(x => x.CountyParishHoldingId).NotEmpty();
        RuleFor(x => x.DelegatingUserId).NotEmpty();
        RuleFor(x => x.DelegatedUserId).NotEmpty();
        RuleFor(x => x.DelegatedUserRoleId).NotEmpty();
        RuleFor(x => x.DelegatedUserEmail).NotEmpty().EmailAddress();
    }
}
