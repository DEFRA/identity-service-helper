// <copyright file="DeleteStrategyBuilder.cs" company="Defra">
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

public partial class DeleteStrategyBuilder<TService, TEntity>
    : StrategyBuilderBase<TService, DeleteStrategyBuilder<TService, TEntity>>
    where TService : class
    where TEntity : class
{
    private IGettable<TEntity>? GettableRepository { get; set; }

    private IDeletable<TEntity>? DeletableRepository { get; set; }

    private ILoggableById? Request { get; set; }

    private Expression<Func<TEntity, bool>>? EntityFilter { get; set; }

    private ExistenceRulesBuilder<TService, TEntity>? ExistenceRulesBuilder { get; set; }

    public DeleteStrategyBuilder<TService, TEntity> WithRepository<TRepository>(TRepository repository)
        where TRepository : IGettable<TEntity>, IDeletable<TEntity>
    {
        GettableRepository = repository;
        DeletableRepository = repository;
        return this;
    }

    public DeleteStrategyBuilder<TService, TEntity> WithRequest(ILoggableById request)
    {
        Request = request;
        return this;
    }

    public DeleteStrategyBuilder<TService, TEntity> WithEntityFilter(Expression<Func<TEntity, bool>> entityFilter)
    {
        EntityFilter = entityFilter;
        return this;
    }

    public DeleteStrategyBuilder<TService, TEntity> WithExistenceRules(Action<ExistenceRulesBuilder<TService, TEntity>> builder)
    {
        ExistenceRulesBuilder = new ExistenceRulesBuilder<TService, TEntity>();

        builder(ExistenceRulesBuilder);

        return this;
    }

    public async Task<bool> Execute()
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

        if (DeletableRepository == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.DeletableRepositoryRequired);
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

        LogExecutingAction(Logger, ActionDescription.ToLowerInvariant(), EntityDescription.ToLowerInvariant(), Request.GetLoggableId(), OperatorContext.OperatorId);

        InvokeBeforeExecuteAction();

        await ExecuteRequestValidation();

        var entityToDelete = await GettableRepository.GetSingle(EntityFilter, CancellationToken.Value);

        if (entityToDelete == null)
        {
            LogEntityWithIdNotFound(Logger, EntityDescription, Request.GetLoggableId());

            throw new NotFoundException($"{EntityDescription} not found.");
        }

        ExistenceRulesBuilder?.Validate(Request, entityToDelete, EntityDescription, Logger);

        var successfullyDeleted = await DeletableRepository.Delete(EntityFilter, OperatorContext.OperatorId, CancellationToken.Value);

        LogSuccessfullyExecutedAction(Logger, ActionDescription.ToLowerInvariant(), EntityDescription.ToLowerInvariant(), Request.GetLoggableId(), OperatorContext.OperatorId);

        return successfullyDeleted;
    }
}
