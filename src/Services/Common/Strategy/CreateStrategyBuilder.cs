// <copyright file="CreateStrategyBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Strategy;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Services.Common.Strategy.Base;
using Defra.Identity.Services.Common.Strategy.Constants;
using Defra.Identity.Services.Common.Strategy.Rules;

[ExcludeFromCodeCoverage]
public partial class CreateStrategyBuilder<TService, TEntity>
    : StrategyBuilderBase<TService, CreateStrategyBuilder<TService, TEntity>>
    where TService : class
    where TEntity : class
{
    private ICreatable<TEntity>? CreatableRepository { get; set; }

    private Func<TEntity>? CreateAction { get; set; }

    private Func<TEntity, Task>? AfterExecuteAction { get; set; }

    private ReferenceRulesBuilder<TService>? ReferenceRulesBuilder { get; set; }

    public CreateStrategyBuilder<TService, TEntity> WithRepository(ICreatable<TEntity> repository)
    {
        CreatableRepository = repository;
        return this;
    }

    public CreateStrategyBuilder<TService, TEntity> WithReferenceRules(Action<ReferenceRulesBuilder<TService>> builder)
    {
        ReferenceRulesBuilder = new ReferenceRulesBuilder<TService>();

        builder(ReferenceRulesBuilder);

        return this;
    }

    public CreateStrategyBuilder<TService, TEntity> WithCreate(Func<TEntity> createAction)
    {
        CreateAction = createAction;

        return this;
    }

    public CreateStrategyBuilder<TService, TEntity> WithAfterExecute(Func<TEntity, Task> afterExecuteAction)
    {
        AfterExecuteAction = afterExecuteAction;

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

        if (CreatableRepository == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.CreatableRepositoryRequired);
        }

        if (EntityDescription == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.PrimaryEntityDescriptionRequired);
        }

        if (ActionDescription == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.ActionDescriptionRequired);
        }

        if (CreateAction == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.CreateActionRequired);
        }

        EnsureOperatorIdProvided();

        LogExecutingAction(
            Logger,
            ActionDescription.ToLowerInvariant(),
            EntityDescription.ToLowerInvariant(),
            OperatorContext.OperatorId);

        InvokeBeforeExecuteAction();

        await ExecuteRequestValidation();

        if (ReferenceRulesBuilder != null)
        {
            await ReferenceRulesBuilder.Validate(ActionDescription, EntityDescription, Logger, CancellationToken.Value);
        }

        var entityToCreate = CreateAction();

        var createdEntity = await CreatableRepository.Create(entityToCreate, CancellationToken.Value);

        if (AfterExecuteAction != null)
        {
            await AfterExecuteAction.Invoke(createdEntity);
        }

        LogSuccessfullyExecutedAction(
            Logger,
            ActionDescription.ToLowerInvariant(),
            EntityDescription.ToLowerInvariant(),
            OperatorContext.OperatorId);

        return createdEntity;
    }

    public async Task<TResult> ExecuteAndMap<TResult>(Func<TEntity, TResult> map)
        where TResult : class
    {
        var entity = await Execute();

        return map(entity);
    }
}
