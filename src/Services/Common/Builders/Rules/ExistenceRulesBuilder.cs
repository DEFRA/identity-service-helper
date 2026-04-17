// <copyright file="ExistenceRulesBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Rules;

using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.Common.Builders.Predicates;
using Defra.Identity.Services.Common.Builders.Predicates.Models;
using Microsoft.Extensions.Logging;

public class ExistenceRulesBuilder<TService, TEntity>
    where TService : class
    where TEntity : class
{
    private readonly EntityPredicatesBuilder<TEntity> entityPredicatesBuilder = new EntityPredicatesBuilder<TEntity>();

    public ExistenceRulesBuilder<TService, TEntity> Add(Func<TEntity, bool> expression, string description)
    {
        entityPredicatesBuilder.Add(expression, description);

        return this;
    }

    public ExistenceRulesBuilder<TService, TEntity> Add(EntityPredicate<TEntity> predicate)
    {
        entityPredicatesBuilder.Add(predicate);

        return this;
    }

    public void Validate(IOperationById request, TEntity entityToValidate, string primaryEntityDescription, ILogger<TService> logger)
    {
        foreach (var rule in entityPredicatesBuilder.Predicates)
        {
            var validAgainstExistenceRule = rule.Predicate(entityToValidate);

            if (!validAgainstExistenceRule)
            {
                logger.LogWarning("{EntityDescription} with id {Id} not found", primaryEntityDescription, request.Id);

                throw new NotFoundException($"{primaryEntityDescription} not found.");
            }
        }
    }
}
