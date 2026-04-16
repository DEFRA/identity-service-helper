// <copyright file="BusinessRulesBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Rules;

using Defra.Identity.Requests.Common;
using Defra.Identity.Services.Common.Builders.Predicates;
using Defra.Identity.Services.Common.Builders.Predicates.Models;
using Defra.Identity.Services.Common.Exceptions;
using Microsoft.Extensions.Logging;

public class BusinessRulesBuilder<TService, TEntity>
    where TService : class
    where TEntity : class
{
    private readonly EntityPredicatesBuilder<TEntity> entityPredicatesBuilder = new EntityPredicatesBuilder<TEntity>();

    public BusinessRulesBuilder<TService, TEntity> Add(Func<TEntity, bool> expression, string description, string? errorMessage = null)
    {
        entityPredicatesBuilder.Add(expression, description, errorMessage);

        return this;
    }

    public BusinessRulesBuilder<TService, TEntity> Add(EntityPredicate<TEntity> predicate)
    {
        entityPredicatesBuilder.Add(predicate);

        return this;
    }

    public void Validate(IOperationById request, TEntity entityToValidate, string actionDescription, string primaryEntityDescription, ILogger<TService> logger)
    {
        foreach (var rule in entityPredicatesBuilder.Predicates)
        {
            var validAgainstBusinessRule = rule.Predicate(entityToValidate);

            if (!validAgainstBusinessRule)
            {
                logger.LogWarning(
                    "Execute {ActionDescription} {EntityDescription} with id {Id} failed business rule '{Description}'",
                    actionDescription.ToLowerInvariant(),
                    primaryEntityDescription.ToLowerInvariant(),
                    request.Id,
                    rule.Description);

                throw new BusinessRuleException(rule.Description);
            }
        }
    }
}
