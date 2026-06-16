// <copyright file="ICreatable.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Common.Composites;

using Defra.Identity.Repositories.Common.Composites.Base;

public interface ICreatable<TEntity> : ICapability
    where TEntity : class
{
    Task<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default);
}
