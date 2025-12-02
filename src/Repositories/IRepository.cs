// <copyright file="IRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories;

using System.Linq.Expressions;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task<List<TEntity>> Get(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default);

    Task<TEntity> Update(TEntity entity);

    Task<bool> Delete(Func<TEntity, bool> predicate);
}
