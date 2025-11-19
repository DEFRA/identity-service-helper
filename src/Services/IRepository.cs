// <copyright file="IRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Services;

using System.Linq.Expressions;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task<List<TEntity>> GetAll();

    Task<TEntity?> Get(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity> Create(TEntity entity);

    Task<TEntity> Update(TEntity entity);

    Task<bool> Delete(Func<TEntity, bool> predicate);
}
