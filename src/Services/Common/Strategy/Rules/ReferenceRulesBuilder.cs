// <copyright file="ReferenceRulesBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Strategy.Rules;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Services.Common.Strategy.Rules.Models;
using Microsoft.Extensions.Logging;

[ExcludeFromCodeCoverage]
public partial class ReferenceRulesBuilder<TService>
    where TService : class
{
    private List<IReferenceRule<TService>> ReferenceRules { get; } = [];

    public ReferenceRulesBuilder<TService> Add<TEntity>(
        IGettable<TEntity> repository,
        Expression<Func<TEntity, bool>> predicate,
        string description)
        where TEntity : class
    {
        ReferenceRules.Add(new ReferenceRule<TService, TEntity>(repository, predicate, description));

        return this;
    }

    public async Task Validate(
        string actionDescription,
        string primaryEntityDescription,
        ILogger<TService> logger,
        CancellationToken cancellationToken)
    {
        foreach (var rule in ReferenceRules)
        {
            await rule.Validate(actionDescription, primaryEntityDescription, logger, cancellationToken);
        }
    }
}
