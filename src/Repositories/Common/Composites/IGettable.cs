// <copyright file="IGettable.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Common.Composites;

using System.Linq.Expressions;

public interface IGettable<TEntity>
    where TEntity : class
{
    Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
