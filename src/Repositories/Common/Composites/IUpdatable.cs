// <copyright file="IUpdatable.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Common.Composites;

public interface IUpdatable<TEntity>
    where TEntity : class
{
    Task<TEntity> Update(TEntity entity, CancellationToken cancellationToken = default);
}
