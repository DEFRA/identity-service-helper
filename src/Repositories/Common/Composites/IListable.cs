// <copyright file="IListable.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Common.Composites;

using System.Linq.Expressions;

public interface IListable<TEntity>
    where TEntity : class
{
    Task<List<TEntity>> GetList(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
