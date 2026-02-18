// <copyright file="PagedEntitiesExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Extensions;

using Defra.Identity.Repositories.Common;
using Defra.Identity.Responses.Common;

public static class PagedEntitiesExtensions
{
    public static PagedResults<T> ToPagedResults<T, TEntity>(this PagedEntities<TEntity> pagedEntities, Func<TEntity, T> mapper)
        => new(pagedEntities.Items.Select(mapper), pagedEntities.TotalCount, pagedEntities.TotalPages, pagedEntities.PageNumber, pagedEntities.PageSize);
}
