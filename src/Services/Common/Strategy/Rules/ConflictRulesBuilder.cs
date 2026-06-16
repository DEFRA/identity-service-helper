// <copyright file="ConflictRulesBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Strategy.Rules;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Services.Common.Exceptions;
using Defra.Identity.Services.Common.Strategy.Rules.Models;
using Defra.Identity.Services.Common.Strategy.Rules.Predicates;
using Microsoft.Extensions.Logging;

[ExcludeFromCodeCoverage]
public class ConflictRulesBuilder<TService, TEntity>
    where TService : class
    where TEntity : class
{
    private readonly EntityPredicatesBuilder<TEntity> entityPredicatesBuilder = new EntityPredicatesBuilder<TEntity>();

    public ConflictRulesBuilder<TService, TEntity> Add(Func<TEntity, bool> expression, string description, string? errorMessage = null)
    {
        entityPredicatesBuilder.Add(expression, description, errorMessage);

        return this;
    }

    public ConflictRulesBuilder<TService, TEntity> Add(EntityPredicate<TEntity> predicate)
    {
        entityPredicatesBuilder.Add(predicate);

        return this;
    }

    public void Validate(ILoggableById request, TEntity entityToValidate, string actionDescription, string primaryEntityDescription, ILogger<TService> logger)
    {
        foreach (var rule in entityPredicatesBuilder.Predicates)
        {
            var validAgainstBusinessRule = rule.Predicate(entityToValidate);

            if (!validAgainstBusinessRule)
            {
                logger.LogWarning(
                    "Execute {ActionDescription} [{EntityDescription}] with id {Id} failed conflict rule '{Description}'",
                    actionDescription.ToLowerInvariant(),
                    primaryEntityDescription.ToLowerInvariant(),
                    request.GetLoggableId(),
                    rule.Description);

                throw new ConflictException(rule.Description);
            }
        }
    }
}
