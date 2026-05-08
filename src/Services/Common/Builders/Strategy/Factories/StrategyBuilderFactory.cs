// <copyright file="StrategyBuilderFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy.Factories;

using Defra.Identity.Services.Common.Builders.Strategy.Base;
using Defra.Identity.Services.Common.Context;
using Microsoft.Extensions.Logging;

public class StrategyBuilderFactory<TService> : IStrategyBuilderFactory<TService>
    where TService : class
{
    private ILogger<TService>? DefaultLogger { get; set; }

    private IOperatorContext? DefaultOperatorContext { get; set; }

    private string? DefaultEntityDescription { get; set; }

    public IStrategyBuilderFactory<TService> WithDefaultLogger(ILogger<TService> logger)
    {
        this.DefaultLogger = logger;
        return this;
    }

    public IStrategyBuilderFactory<TService> WithDefaultOperatorContext(IOperatorContext operatorContext)
    {
        this.DefaultOperatorContext = operatorContext;
        return this;
    }

    public IStrategyBuilderFactory<TService> WithDefaultEntityDescription(string entityDescription)
    {
        this.DefaultEntityDescription = entityDescription;
        return this;
    }

    public CreateStrategyBuilder<TService, TEntity> BuildCreateStrategy<TEntity>()
        where TEntity : class
    {
        var createStrategyBuilder = new CreateStrategyBuilder<TService, TEntity>();

        AttachDefaults(createStrategyBuilder);

        return createStrategyBuilder;
    }

    public UpdateStrategyBuilder<TService, TEntity> BuildUpdateStrategy<TEntity>()
        where TEntity : class
    {
        var updateStrategyBuilder = new UpdateStrategyBuilder<TService, TEntity>();

        AttachDefaults(updateStrategyBuilder);

        return updateStrategyBuilder;
    }

    public DeleteStrategyBuilder<TService, TEntity> BuildDeleteStrategy<TEntity>()
        where TEntity : class
    {
        var deleteStrategyBuilder = new DeleteStrategyBuilder<TService, TEntity>();

        AttachDefaults(deleteStrategyBuilder);

        return deleteStrategyBuilder;
    }

    public GetStrategyBuilder<TService, TEntity> BuildGetStrategy<TEntity>()
        where TEntity : class
    {
        var getStrategyBuilder = new GetStrategyBuilder<TService, TEntity>();

        AttachDefaults(getStrategyBuilder);

        return getStrategyBuilder;
    }

    public GetListStrategyBuilder<TService, TEntity> BuildGetListStrategy<TEntity>()
        where TEntity : class
    {
        var getListStrategyBuilder = new GetListStrategyBuilder<TService, TEntity>();

        AttachDefaults(getListStrategyBuilder);

        return getListStrategyBuilder;
    }

    public GetPagedStrategyBuilder<TService, TEntity> BuildGetPagedStrategy<TEntity>()
        where TEntity : class
    {
        var getPagedStrategyBuilder = new GetPagedStrategyBuilder<TService, TEntity>();

        AttachDefaults(getPagedStrategyBuilder);

        return getPagedStrategyBuilder;
    }

    private void AttachDefaults<TBuilder>(StrategyBuilderBase<TService, TBuilder> strategyBuilder)
        where TBuilder : StrategyBuilderBase<TService, TBuilder>
    {
        if (DefaultLogger != null)
        {
            strategyBuilder.WithLogger(DefaultLogger);
        }

        if (DefaultOperatorContext != null)
        {
            strategyBuilder.WithOperatorContext(DefaultOperatorContext);
        }

        if (DefaultEntityDescription != null)
        {
            strategyBuilder.WithEntityDescription(DefaultEntityDescription);
        }
    }
}
