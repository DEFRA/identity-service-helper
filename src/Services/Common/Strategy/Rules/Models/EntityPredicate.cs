// <copyright file="EntityPredicate.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Strategy.Rules.Models;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class EntityPredicate<TEntity>
    where TEntity : class
{
    public EntityPredicate(Func<TEntity, bool> predicate, string description, string? errorMessage = null)
    {
        this.Predicate = predicate;
        this.Description = description;
        this.ErrorMessage = errorMessage;
    }

    public Func<TEntity, bool> Predicate { get; }

    public string Description { get; }

    public string? ErrorMessage { get; }
}
