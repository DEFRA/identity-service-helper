// <copyright file="IRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories;

using System.Linq.Expressions;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default);

    Task<TEntity> Update(TEntity entity, CancellationToken cancellationToken = default);

    Task<bool> Delete(Expression<Func<TEntity, bool>> predicate,  Guid operatorId, CancellationToken cancellationToken = default);
}
