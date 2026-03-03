// <copyright file="IDeleteRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories;

using System.Linq.Expressions;

public interface IDeleteRepository<TEntity>
    where TEntity : class
{
    Task<bool> Delete(
        Expression<Func<TEntity, bool>> predicate,
        Guid operatorId,
        CancellationToken cancellationToken = default);
}
