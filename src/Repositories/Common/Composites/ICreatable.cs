// <copyright file="ICreatable.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Common.Composites;

public interface ICreatable<TEntity>
    where TEntity : class
{
    Task<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default);
}
