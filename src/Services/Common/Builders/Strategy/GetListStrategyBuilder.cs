// <copyright file="GetListStrategyBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using System.Linq.Expressions;
using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Services.Common.Builders.Strategy.Base;
using Defra.Identity.Services.Common.Builders.Strategy.Constants;
using Microsoft.Extensions.Logging;

public class GetListStrategyBuilder<TService, TEntity> : StrategyBuilderBase<TService,
    GetListStrategyBuilder<TService, TEntity>>
    where TService : class
    where TEntity : class
{
    private IListable<TEntity>? ListableRepository { get; set; }

    private Expression<Func<TEntity, bool>>? EntityFilter { get; set; }

    public GetListStrategyBuilder<TService, TEntity> WithRepository<TRepository>(TRepository repository)
        where TRepository : IListable<TEntity>
    {
        ListableRepository = repository;
        return this;
    }

    public GetListStrategyBuilder<TService, TEntity> WithEntityFilter(Expression<Func<TEntity, bool>> entityFilter)
    {
        EntityFilter = entityFilter;
        return this;
    }

    public async Task<List<TResult>> ExecuteAndMap<TResult>(Func<TEntity, TResult> map)
        where TResult : class
    {
        if (Logger == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.LoggerRequired);
        }

        if (CancellationToken == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.CancellationTokenRequired);
        }

        if (ListableRepository == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.ListableRepositoryRequired);
        }

        if (PrimaryEntityDescription == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.PrimaryEntityDescriptionRequired);
        }

        if (ActionDescription == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.ActionDescriptionRequired);
        }

        if (EntityFilter == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.EntityFilterRequired);
        }

        Logger.LogInformation(
            "Executing {ActionDescription} {EntityDescription}",
            ActionDescription.ToLowerInvariant(),
            PrimaryEntityDescription.ToLowerInvariant());

        ExecuteSetup();

        await ExecuteRequestValidation();

        var entities = await ListableRepository.GetList(EntityFilter, CancellationToken.Value);

        var mappedEntities = entities.Select(map).ToList();

        Logger.LogInformation(
            "Successfully executed {ActionDescription} {EntityDescription}",
            ActionDescription.ToLowerInvariant(),
            PrimaryEntityDescription.ToLowerInvariant());

        return mappedEntities;
    }
}
