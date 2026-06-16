// <copyright file="IGettable.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Common.Composites;

using System.Linq.Expressions;
using Defra.Identity.Repositories.Common.Composites.Base;

public interface IGettable<TEntity> : ICapability
    where TEntity : class
{
    Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
