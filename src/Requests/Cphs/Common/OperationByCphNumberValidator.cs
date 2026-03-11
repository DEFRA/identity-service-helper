// <copyright file="OperationByCphNumberValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Cphs.Common;

using FluentValidation;

public class OperationByCphNumberValidator : AbstractValidator<IOperationByCphNumber>
{
    public OperationByCphNumberValidator()
    {
        RuleFor(x => x.County).GreaterThanOrEqualTo(0).LessThanOrEqualTo(99);
        RuleFor(x => x.Parish).GreaterThanOrEqualTo(0).LessThanOrEqualTo(999);
        RuleFor(x => x.Holding).GreaterThanOrEqualTo(0).LessThanOrEqualTo(9999);
    }
}
