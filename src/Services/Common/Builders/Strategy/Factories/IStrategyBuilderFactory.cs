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

    IStrategyBuilderFactory<TService> WithDefaultPrimaryEntityDescription(string primaryEntityDescription);

    CreateStrategyBuilder<TService, TEntity> BuildCreateStrategy<TEntity>()
        where TEntity : class;

    UpdateStrategyBuilder<TService, TEntity> BuildUpdateStrategy<TEntity>()
        where TEntity : class;

    DeleteStrategyBuilder<TService, TEntity> BuildDeleteStrategy<TEntity>()
        where TEntity : class;

    GetStrategyBuilder<TService, TEntity> BuildGetStrategy<TEntity>()
        where TEntity : class;

    GetListStrategyBuilder<TService, TEntity> BuildGetListStrategy<TEntity>()
        where TEntity : class;

    GetPagedStrategyBuilder<TService, TEntity> BuildGetPagedStrategy<TEntity>()
        where TEntity : class;

    GetAssociationsListStrategyBuilder<TService, TPrimary, TAssociation> BuildGetAssociationsListStrategy<TPrimary, TAssociation>()
        where TPrimary : class
        where TAssociation : class;

    GetAssociationsPagedStrategyBuilder<TService, TPrimary, TAssociation> BuildGetAssociationsPagedStrategy<TPrimary, TAssociation>()
        where TPrimary : class
        where TAssociation : class;
}
