// <copyright file="QueryablePagingExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Extensions;

using System.Linq.Expressions;
using Defra.Identity.Repositories.Common;

public static class QueryablePagingExtensions
{
    public static async Task<PagedEntities<TEntity>> ToPaged<TEntity, TOrderBy>(
        this IQueryable<TEntity> query,
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, TOrderBy>> orderBy,
        bool orderByDescending,
        CancellationToken cancellationToken = default)
    {
        query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (totalCount + pageSize - 1) / pageSize;

        var resultsList = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedEntities<TEntity>(resultsList, totalCount, totalPages, pageNumber, pageSize);
    }
}
