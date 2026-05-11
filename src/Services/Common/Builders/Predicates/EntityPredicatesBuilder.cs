// <copyright file="EntityPredicatesBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Predicates;

using Defra.Identity.Services.Common.Builders.Predicates.Models;

public class EntityPredicatesBuilder<TEntity>
    where TEntity : class
{
    public List<EntityPredicate<TEntity>> Predicates { get; } = [];

    public EntityPredicatesBuilder<TEntity> Add(Func<TEntity, bool> predicate, string description, string? errorMessage = null)
    {
        Predicates.Add(new EntityPredicate<TEntity>(predicate, description, errorMessage));

        return this;
    }

    public EntityPredicatesBuilder<TEntity> Add(EntityPredicate<TEntity> predicate)
    {
        Predicates.Add(predicate);

        return this;
    }
}
