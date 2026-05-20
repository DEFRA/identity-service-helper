// <copyright file="IStrategyBuilderFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy.Factories;

using Defra.Identity.Services.Common.Context;
using Microsoft.Extensions.Logging;

public interface IStrategyBuilderFactory<TService>
    where TService : class
{
    IStrategyBuilderFactory<TService> WithDefaultLogger(ILogger<TService> logger);

    IStrategyBuilderFactory<TService> WithDefaultOperatorContext(IOperatorContext operatorContext);

    IStrategyBuilderFactory<TService> WithDefaultEntityDescription(string entityDescription);

    CreateStrategyBuilder<TService, TEntity> BuildCreateStrategy<TEntity>()
        where TEntity : class;

    UpdateStrategyBuilder<TService, TEntity> BuildUpdateStrategy<TEntity>()
        where TEntity : class;

    UpsertStrategyBuilder<TService, TEntity> BuildUpsertStrategy<TEntity>()
        where TEntity : class;

    GetStrategyBuilder<TService, TEntity> BuildGetStrategy<TEntity>()
        where TEntity : class;

    GetListStrategyBuilder<TService, TEntity> BuildGetListStrategy<TEntity>()
        where TEntity : class;

    GetPagedStrategyBuilder<TService, TEntity> BuildGetPagedStrategy<TEntity>()
        where TEntity : class;
}
