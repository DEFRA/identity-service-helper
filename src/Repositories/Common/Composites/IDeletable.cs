// <copyright file="IDeletable.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Common.Composites;

using System.Linq.Expressions;

public interface IDeletable<TEntity>
    where TEntity : class
{
    Task<bool> Delete(Expression<Func<TEntity, bool>> predicate, Guid operatorId, CancellationToken cancellationToken = default);
}
