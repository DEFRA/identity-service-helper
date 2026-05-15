// <copyright file="GetStrategyBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.Common.Builders.Rules;
using Defra.Identity.Services.Common.Builders.Strategy.Base;
using Defra.Identity.Services.Common.Builders.Strategy.Constants;

public partial class GetStrategyBuilder<TService, TEntity>
    : StrategyBuilderBase<TService, GetStrategyBuilder<TService, TEntity>>
    where TService : class
    where TEntity : class
{
    private IGettable<TEntity>? GettableRepository { get; set; }

    private ILoggableById? Request { get; set; }

    private Expression<Func<TEntity, bool>>? EntityFilter { get; set; }

    private ExistenceRulesBuilder<TService, TEntity>? ExistenceRulesBuilder { get; set; }

    public GetStrategyBuilder<TService, TEntity> WithRepository<TRepository>(TRepository repository)
        where TRepository : IGettable<TEntity>, IUpdatable<TEntity>
    {
        GettableRepository = repository;
        return this;
    }

    public GetStrategyBuilder<TService, TEntity> WithRequest(ILoggableById request)
    {
        Request = request;
        return this;
    }

    public GetStrategyBuilder<TService, TEntity> WithEntityFilter(Expression<Func<TEntity, bool>> entityFilter)
    {
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

        if (EntityDescription == null)
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

        LogExecutingAction(Logger, ActionDescription.ToLowerInvariant(), EntityDescription.ToLowerInvariant(), Request.GetLoggableId());

        InvokeBeforeExecuteAction();

        await ExecuteRequestValidation();

        var entity = await GettableRepository.GetSingle(EntityFilter, CancellationToken.Value);

        if (entity == null)
        {
            LogEntityWithIdNotFound(Logger, EntityDescription, Request.GetLoggableId());

            throw new NotFoundException($"{EntityDescription} not found.");
        }

        ExistenceRulesBuilder?.Validate(Request, entity, EntityDescription, Logger);

        var mappedEntity = map(entity);

        LogSuccessfullyExecutedAction(Logger, ActionDescription.ToLowerInvariant(), EntityDescription.ToLowerInvariant(), Request.GetLoggableId());

        return mappedEntity;
    }
}
