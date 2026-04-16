// <copyright file="GetStrategyBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using System.Linq.Expressions;
using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Requests.Common;
using Defra.Identity.Services.Common.Builders.Rules;
using Defra.Identity.Services.Common.Builders.Strategy.Base;
using Defra.Identity.Services.Common.Builders.Strategy.Constants;
using Microsoft.Extensions.Logging;

public class GetStrategyBuilder<TService, TEntity> : StrategyBuilderBase<TService, GetStrategyBuilder<TService, TEntity>>
    where TService : class
    where TEntity : class
{
    private IGettable<TEntity>? GettableRepository { get; set; }

    private IOperationById? Request { get; set; }

    private Expression<Func<TEntity, bool>>? EntityFilter { get; set; }

    private ExistenceRulesBuilder<TService, TEntity>? ExistenceRulesBuilder { get; set; }

    public GetStrategyBuilder<TService, TEntity> WithRepository<TRepository>(TRepository repository)
        where TRepository : IGettable<TEntity>, IUpdatable<TEntity>
    {
        GettableRepository = repository;
        return this;
    }

    public GetStrategyBuilder<TService, TEntity> WithRequestAndEntityFilter(IOperationById request, Expression<Func<TEntity, bool>> entityFilter)
    {
        Request = request;
        EntityFilter = entityFilter;
        return this;
    }

    public GetStrategyBuilder<TService, TEntity> WithExistenceRules(Action<ExistenceRulesBuilder<TService, TEntity>> builder)
    {
        ExistenceRulesBuilder = new ExistenceRulesBuilder<TService, TEntity>();

        builder(ExistenceRulesBuilder);

        return this;
    }

    public async Task<TResult> ExecuteAndMap<TResult>(Func<TEntity, TResult> map)
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

        if (GettableRepository == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.GettableRepositoryRequired);
        }

        if (PrimaryEntityDescription == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.PrimaryEntityDescriptionRequired);
        }

        if (ActionDescription == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.ActionDescriptionRequired);
        }

        if (Request == null || EntityFilter == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.RequestAndEntityFilterRequired);
        }

        Logger.LogInformation(
            "Executing {ActionDescription} {EntityDescription} with id {Id}",
            ActionDescription.ToLowerInvariant(),
            PrimaryEntityDescription.ToLowerInvariant(),
            Request.Id);

        await ExecuteRequestValidation();

        var entity = await GettableRepository.GetSingle(EntityFilter, CancellationToken.Value);

        if (entity == null)
        {
            Logger.LogWarning("{EntityDescription} with id {Id} not found", PrimaryEntityDescription, Request.Id);

            throw new NotFoundException($"{PrimaryEntityDescription} not found.");
        }

        ExistenceRulesBuilder?.Validate(Request, entity, PrimaryEntityDescription, Logger);

        var mappedEntity = map(entity);

        Logger.LogInformation(
            "Successfully executed {ActionDescription} {EntityDescription} with id {Id}",
            ActionDescription.ToLowerInvariant(),
            PrimaryEntityDescription.ToLowerInvariant(),
            Request.Id);

        return mappedEntity;
    }
}
