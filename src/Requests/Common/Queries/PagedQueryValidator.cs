// <copyright file="PagedQueryValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Common.Queries;

using FluentValidation;

public class PagedQueryValidator : AbstractValidator<PagedQuery>
{
    private const int MaxPageSize = 500;

    public PagedQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(MaxPageSize);
    }
}
