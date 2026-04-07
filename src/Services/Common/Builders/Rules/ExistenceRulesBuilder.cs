// <copyright file="ExistenceRulesBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Rules;

using System.Linq.Expressions;
using Defra.Identity.Services.Common.Builders.Predicates;
using Defra.Identity.Services.Common.Builders.Predicates.Models;

public class ExistenceRulesBuilder<TEntity>
    where TEntity : class
{
    private readonly EntityPredicatesBuilder<TEntity> entityPredicatesBuilder = new EntityPredicatesBuilder<TEntity>();

    public List<EntityPredicate<TEntity>> ExistenceRules => entityPredicatesBuilder.Predicates;

    public ExistenceRulesBuilder<TEntity> Add(Func<TEntity, bool> expression, string description)
    {
        entityPredicatesBuilder.Add(expression, description);

        return this;
    }

    public ExistenceRulesBuilder<TEntity> Add(EntityPredicate<TEntity> predicate)
    {
        entityPredicatesBuilder.Add(predicate);

        return this;
    }
}
