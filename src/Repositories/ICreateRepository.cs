// <copyright file="ICreateRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories;

public interface ICreateRepository<TEntity>
    where TEntity : class
{
    Task<TEntity> Create(
        TEntity entity,
        CancellationToken cancellationToken = default);
}
