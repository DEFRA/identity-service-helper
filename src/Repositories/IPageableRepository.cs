// <copyright file="IPageableRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories;

using System.Linq.Expressions;
using Defra.Identity.Repositories.Common;

public interface IPageableRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<PagedEntities<TEntity>> GetPaged<TOrderBy>(
        Expression<Func<TEntity, bool>> predicate,
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, TOrderBy>> orderBy,
        bool orderByDescending,
        CancellationToken cancellationToken = default);

    Task<TEntity> Create(TEntity entity, Guid operatorId, CancellationToken cancellationToken = default);

    Task<TEntity> Update(TEntity entity, Guid operatorId, CancellationToken cancellationToken = default);

    Task<TEntity?> Delete(Expression<Func<TEntity, bool>> predicate, Guid operatorId, CancellationToken cancellationToken = default);
}
