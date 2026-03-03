// <copyright file="IGetSingleRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories;

using System.Linq.Expressions;

public interface IGetSingleRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> GetSingle(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}
