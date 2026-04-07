// <copyright file="IStrategyBuilderFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy.Factories;

using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Services.Common.Context;
using Microsoft.Extensions.Logging;

public interface IStrategyBuilderFactory<TService, TEntity>
    where TService : class
    where TEntity : class
{
    IStrategyBuilderFactory<TService, TEntity> WithLogger(ILogger<TService> logger);

    IStrategyBuilderFactory<TService, TEntity> WithOperatorContext(IOperatorContext operatorContext);

    IStrategyBuilderFactory<TService, TEntity> WithEntityDescription(string entityDescription);

    public UpdateStrategyBuilder<TService, TRepository, TEntity> BuildUpdateStrategy<TRepository>(TRepository? repository = default, string? actionDescription = null)
        where TRepository : IGettable<TEntity>, IUpdatable<TEntity>;

    public CreateStrategyBuilder<TService, TRepository, TEntity> BuildCreateStrategy<TRepository>(TRepository? repository = default, string? actionDescription = null)
        where TRepository : ICreatable<TEntity>;
}
