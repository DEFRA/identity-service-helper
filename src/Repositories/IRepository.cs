// <copyright file="IRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories;

using Defra.Identity.Repositories.Common.Composites;

public interface IRepository<TEntity> :
    IGettable<TEntity>,
    IListable<TEntity>,
    ICreatable<TEntity>,
    IUpdatable<TEntity>
    where TEntity : class;
