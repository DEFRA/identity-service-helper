// <copyright file="UpdateStrategyBuilder.cs" company="Defra">
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

public partial class UpdateStrategyBuilder<TService, TEntity>
    : StrategyBuilderBase<TService, UpdateStrategyBuilder<TService, TEntity>>
    where TService : class
    where TEntity : class
{
    private IGettable<TEntity>? GettableRepository { get; set; }

    private IUpdatable<TEntity>? UpdateableRepository { get; set; }

    private ILoggableById? Request { get; set; }

    private Expression<Func<TEntity, bool>>? EntityFilter { get; set; }

    private Action<TEntity>? UpdateAction { get; set; }

    private ExistenceRulesBuilder<TService, TEntity>? ExistenceRulesBuilder { get; set; }

    private ReferenceRulesBuilder<TService>? ReferenceRulesBuilder { get; set; }

    private ConflictRulesBuilder<TService, TEntity>? ConflictRulesBuilder { get; set; }

    private BusinessRulesBuilder<TService, TEntity>? BusinessRulesBuilder { get; set; }

    public UpdateStrategyBuilder<TService, TEntity> WithRepository<TRepository>(TRepository repository)
        where TRepository : IGettable<TEntity>, IUpdatable<TEntity>
    {
        GettableRepository = repository;
        UpdateableRepository = repository;
        return this;
    }

    public UpdateStrategyBuilder<TService, TEntity> WithRequest(ILoggableById request)
    {
        Request = request;
        return this;
    }

    public UpdateStrategyBuilder<TService, TEntity> WithEntityFilter(Expression<Func<TEntity, bool>> entityFilter)
    {
        EntityFilter = entityFilter;
        return this;
    }

    public UpdateStrategyBuilder<TService, TEntity> WithExistenceRules(Action<ExistenceRulesBuilder<TService, TEntity>> builder)
    {
        ExistenceRulesBuilder = new ExistenceRulesBuilder<TService, TEntity>();

        builder(ExistenceRulesBuilder);

        return this;
    }

    public UpdateStrategyBuilder<TService, TEntity> WithReferenceRules(Action<ReferenceRulesBuilder<TService>> builder)
    {
        ReferenceRulesBuilder = new ReferenceRulesBuilder<TService>();

        builder(ReferenceRulesBuilder);

        return this;
    }

    public UpdateStrategyBuilder<TService, TEntity> WithConflictRules(Action<ConflictRulesBuilder<TService, TEntity>> builder)
    {
        ConflictRulesBuilder = new ConflictRulesBuilder<TService, TEntity>();

        builder(ConflictRulesBuilder);

        return this;
    }

    public UpdateStrategyBuilder<TService, TEntity> WithBusinessRules(Action<BusinessRulesBuilder<TService, TEntity>> builder)
    {
        BusinessRulesBuilder = new BusinessRulesBuilder<TService, TEntity>();

        builder(BusinessRulesBuilder);

        return this;
    }

    public UpdateStrategyBuilder<TService, TEntity> WithUpdate(Action<TEntity> updateAction)
    {
        UpdateAction = updateAction;

        return this;
    }

    public async Task<TEntity> Execute()
    {
        if (Logger == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.LoggerRequired);
        }

        if (CancellationToken == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.CancellationTokenRequired);
        }

        if (OperatorContext == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.OperatorContextRequired);
        }

        if (GettableRepository == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.GettableRepositoryRequired);
        }

        if (UpdateableRepository == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.UpdatableRepositoryRequired);
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

        if (UpdateAction == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.UpdateActionRequired);
        }

        LogExecutingAction(Logger, ActionDescription.ToLowerInvariant(), EntityDescription.ToLowerInvariant(), Request.GetLoggableId(), OperatorContext.OperatorId);

        InvokeBeforeExecuteAction();

        await ExecuteRequestValidation();

        if (ReferenceRulesBuilder != null)
        {
            await ReferenceRulesBuilder.Validate(ActionDescription, EntityDescription, Logger, CancellationToken.Value);
        }

        var entityToUpdate = await GettableRepository.GetSingle(EntityFilter, CancellationToken.Value);

        if (entityToUpdate == null)
        {
            LogEntityWithIdNotFound(Logger, EntityDescription, Request.GetLoggableId());

            throw new NotFoundException($"{EntityDescription} not found.");
        }

        ExistenceRulesBuilder?.Validate(Request, entityToUpdate, EntityDescription, Logger);
        ConflictRulesBuilder?.Validate(Request, entityToUpdate, ActionDescription, EntityDescription, Logger);
        BusinessRulesBuilder?.Validate(Request, entityToUpdate, ActionDescription, EntityDescription, Logger);

        UpdateAction(entityToUpdate);

        var updatedEntity = await UpdateableRepository.Update(entityToUpdate, CancellationToken.Value);

        LogSuccessfullyExecutedAction(Logger, ActionDescription.ToLowerInvariant(), EntityDescription.ToLowerInvariant(), Request.GetLoggableId(), OperatorContext.OperatorId);

        return updatedEntity;
    }

    public async Task<TResult> ExecuteAndMap<TResult>(Func<TEntity, TResult> map)
        where TResult : class
    {
        var entity = await Execute();

        return map(entity);
    }
}
