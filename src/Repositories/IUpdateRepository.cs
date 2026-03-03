// <copyright file="IUpdateRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories;

public interface IUpdateRepository<TEntity>
    where TEntity : class
{
    Task<TEntity> Update(
        TEntity entity,
        CancellationToken cancellationToken = default);
}
