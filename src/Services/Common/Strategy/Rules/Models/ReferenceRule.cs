// <copyright file="ReferenceRule.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Strategy.Rules.Models;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Repositories.Common.Exceptions;
using Microsoft.Extensions.Logging;

[ExcludeFromCodeCoverage]
public partial class ReferenceRule<TService, TEntity> : IReferenceRule<TService>
    where TService : class
    where TEntity : class
{
    private readonly IGettable<TEntity> repository;

    private readonly Expression<Func<TEntity, bool>> predicate;

    private readonly string description;

    public ReferenceRule(IGettable<TEntity> repository, Expression<Func<TEntity, bool>> predicate, string description)
    {
        this.predicate = predicate;
        this.repository = repository;
        this.description = description;
    }

    public async Task Validate(
        string actionDescription,
        string primaryEntityDescription,
        ILogger<TService> logger,
        CancellationToken cancellationToken)
    {
        var filteredEntity = await repository.GetSingle(predicate, cancellationToken);

        if (filteredEntity == null)
        {
            LogEntityReferenceNotFound(
                logger,
                actionDescription.ToLowerInvariant(),
                primaryEntityDescription.ToLowerInvariant(),
                description);

            throw new NotFoundException(description);
        }
    }
}
