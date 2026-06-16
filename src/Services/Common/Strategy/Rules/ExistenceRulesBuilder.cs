// <copyright file="ExistenceRulesBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Strategy.Rules;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.Common.Strategy.Rules.Models;
using Defra.Identity.Services.Common.Strategy.Rules.Predicates;
using Microsoft.Extensions.Logging;

[ExcludeFromCodeCoverage]
public partial class ExistenceRulesBuilder<TService, TEntity>
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

    public void Validate(ILoggableById request, TEntity entityToValidate, string entityDescription, ILogger<TService> logger)
    {
        foreach (var rule in entityPredicatesBuilder.Predicates)
        {
            var validAgainstExistenceRule = rule.Predicate(entityToValidate);

            if (!validAgainstExistenceRule)
            {
                LogEntityWithIdNotFound(logger, entityDescription, request.GetLoggableId());

                throw new NotFoundException($"{entityDescription} not found");
            }
        }
    }
}
