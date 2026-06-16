// <copyright file="IUpdatable.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Common.Composites;

using Defra.Identity.Repositories.Common.Composites.Base;

public interface IUpdatable<TEntity> : ICapability
    where TEntity : class
{
    Task<TEntity> Update(TEntity entity, CancellationToken cancellationToken = default);
}
