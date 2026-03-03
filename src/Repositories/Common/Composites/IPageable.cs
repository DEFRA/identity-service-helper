// <copyright file="IPageable.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Common.Composites;

using System.Linq.Expressions;

public interface IPageable<TEntity>
    where TEntity : class
{
    Task<PagedEntities<TEntity>> GetPaged<TOrderBy>(
        Expression<Func<TEntity, bool>> predicate,
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, TOrderBy>> orderBy,
        bool orderByDescending,
        CancellationToken cancellationToken = default);
}
