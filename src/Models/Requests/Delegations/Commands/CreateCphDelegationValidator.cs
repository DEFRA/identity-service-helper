// <copyright file="CreateCphDelegationValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Delegations.Commands;

using FluentValidation;

public class CreateCphDelegationValidator : AbstractValidator<CreateCphDelegation>
{
    public CreateCphDelegationValidator()
    {
        RuleFor(x => x.CountyParishHoldingId).NotEqual(Guid.Empty);
        RuleFor(x => x.DelegatingUserId).NotEqual(Guid.Empty);
        RuleFor(x => x.DelegatedUserRoleId).NotEqual(Guid.Empty);
        RuleFor(x => x.DelegatedUserEmail).NotEmpty().MaximumLength(256).EmailAddress();
    }
}
