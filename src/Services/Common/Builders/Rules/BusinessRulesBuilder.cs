// <copyright file="BusinessRulesBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Rules;

using System.Linq.Expressions;
using Defra.Identity.Services.Common.Builders.Predicates;
using Defra.Identity.Services.Common.Builders.Predicates.Models;

public class BusinessRulesBuilder<TEntity>
    where TEntity : class
{
    private readonly EntityPredicatesBuilder<TEntity> entityPredicatesBuilder = new EntityPredicatesBuilder<TEntity>();

    public List<EntityPredicate<TEntity>> BusinessRules => entityPredicatesBuilder.Predicates;

    public BusinessRulesBuilder<TEntity> Add(Func<TEntity, bool> expression, string description, string? errorMessage = null)
    {
        entityPredicatesBuilder.Add(expression, description, errorMessage);

        return this;
    }

    public BusinessRulesBuilder<TEntity> Add(EntityPredicate<TEntity> predicate)
    {
        entityPredicatesBuilder.Add(predicate);

        return this;
    }
}
