// <copyright file="StrategyBuilderFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy.Factories;

using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Services.Common.Builders.Strategy.Base;
using Defra.Identity.Services.Common.Context;
using Microsoft.Extensions.Logging;

public class StrategyBuilderFactory<TService, TEntity> : IStrategyBuilderFactory<TService, TEntity>
    where TService : class
    where TEntity : class
{
    private ILogger<TService>? Logger { get; set; }

    private IOperatorContext? OperatorContext { get; set; }

    private string? EntityDescription { get; set; }

    public IStrategyBuilderFactory<TService, TEntity> WithLogger(ILogger<TService> logger)
    {
        this.Logger = logger;
        return this;
    }

    public IStrategyBuilderFactory<TService, TEntity> WithOperatorContext(IOperatorContext operatorContext)
    {
        this.OperatorContext = operatorContext;
        return this;
    }

    public IStrategyBuilderFactory<TService, TEntity> WithEntityDescription(string entityDescription)
    {
        this.EntityDescription = entityDescription;
        return this;
    }

    public UpdateStrategyBuilder<TService, TRepository, TEntity> BuildUpdateStrategy<TRepository>(TRepository? repository = default, string? actionDescription = null)
        where TRepository : IGettable<TEntity>, IUpdatable<TEntity>
    {
        var updateStrategyBuilder = new UpdateStrategyBuilder<TService, TRepository, TEntity>();

        AttachDefaults(updateStrategyBuilder);

        if (repository != null)
        {
            updateStrategyBuilder.WithRepository(repository);
        }

        if (actionDescription != null)
        {
            updateStrategyBuilder.WithActionDescription(actionDescription);
        }

        return updateStrategyBuilder;
    }

    public CreateStrategyBuilder<TService, TRepository, TEntity> BuildCreateStrategy<TRepository>(TRepository? repository = default, string? actionDescription = null)
        where TRepository : ICreatable<TEntity>
    {
        var createStrategyBuilder = new CreateStrategyBuilder<TService, TRepository, TEntity>();

        AttachDefaults(createStrategyBuilder);

        if (repository != null)
        {
            createStrategyBuilder.WithRepository(repository);
        }

        if (actionDescription != null)
        {
            createStrategyBuilder.WithActionDescription(actionDescription);
        }

        return createStrategyBuilder;
    }

    private void AttachDefaults<TBuilder>(StrategyBuilderBase<TService, TBuilder> strategyBuilder)
        where TBuilder : StrategyBuilderBase<TService, TBuilder>
    {
        if (Logger != null)
        {
            strategyBuilder.WithLogger(Logger);
        }

        if (OperatorContext != null)
        {
            strategyBuilder.WithOperatorContext(OperatorContext);
        }

        if (EntityDescription != null)
        {
            strategyBuilder.WithEntityDescription(EntityDescription);
        }
    }
}
