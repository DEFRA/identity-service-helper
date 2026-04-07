// <copyright file="CreateStrategyBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Services.Common.Builders.Rules;
using Defra.Identity.Services.Common.Builders.Strategy.Base;
using FluentValidation;
using Microsoft.Extensions.Logging;

public class CreateStrategyBuilder<TService, TRepository, TEntity> : StrategyBuilderBase<TService, CreateStrategyBuilder<TService, TRepository, TEntity>>
    where TService : class
    where TRepository : ICreatable<TEntity>
    where TEntity : class
{
    private TRepository? Repository { get; set; }

    private Func<TEntity>? CreateAction { get; set; }

    private ReferenceRulesBuilder? ReferenceRulesBuilder { get; set; }

    public CreateStrategyBuilder<TService, TRepository, TEntity> WithRepository(TRepository repository)
    {
        Repository = repository;
        return this;
    }

    public CreateStrategyBuilder<TService, TRepository, TEntity> WithReferenceRules(Action<ReferenceRulesBuilder> builder)
    {
        ReferenceRulesBuilder = new ReferenceRulesBuilder();

        builder(ReferenceRulesBuilder);

        return this;
    }

    public CreateStrategyBuilder<TService, TRepository, TEntity> WithCreate(Func<TEntity> createAction)
    {
        CreateAction = createAction;

        return this;
    }

    public async Task<TEntity> Execute()
    {
        if (Logger == null)
        {
            throw new InvalidOperationException("Logger must be provided for this operation");
        }

        if (CancellationToken == null)
        {
            throw new InvalidOperationException("Cancellation token must be provided for this operation");
        }

        if (OperatorContext == null)
        {
            throw new InvalidOperationException("Operator context must be provided for this operation");
        }

        if (Repository == null)
        {
            throw new InvalidOperationException("Repository must be provided for this operation");
        }

        if (EntityDescription == null)
        {
            throw new InvalidOperationException("Entity description must be provided for this operation");
        }

        if (ActionDescription == null)
        {
            throw new InvalidOperationException("Action description must be provided for this operation");
        }

        if (CreateAction == null)
        {
            throw new InvalidOperationException("Create action must be provided for this operation");
        }

        Logger.LogInformation(
            "Executing {ActionDescription} {EntityDescription} with by operator {OperatorId}",
            ActionDescription.ToLowerInvariant(),
            EntityDescription.ToLowerInvariant(),
            OperatorContext.OperatorId);

        if (ValidateAction != null)
        {
            var validationResult = await ValidateAction();

            if (!validationResult.IsValid)
            {
                Logger.LogWarning(
                    "Execute {ActionDescription} {EntityDescription} failed basic validation",
                    ActionDescription.ToLowerInvariant(),
                    EntityDescription.ToLowerInvariant());

                throw new ValidationException(validationResult.Errors);
            }
        }

        if (ReferenceRulesBuilder != null)
        {
            foreach (var rule in ReferenceRulesBuilder.ReferenceRules)
            {
                var validAgainstReferenceRule = await rule.Repository.ValidateReferenceById(rule.Id);

                if (!validAgainstReferenceRule)
                {
                    Logger.LogWarning(
                        "Execute {ActionDescription} {EntityDescription} failed reference rule '{Description}'",
                        ActionDescription.ToLowerInvariant(),
                        EntityDescription.ToLowerInvariant(),
                        rule.Description);

                    throw new NotFoundException(rule.Description);
                }
            }
        }

        var entityToCreate = CreateAction();

        var createdEntity = await Repository.Create(entityToCreate, CancellationToken.Value);

        Logger.LogInformation(
            "Successfully executed {ActionDescription} {EntityDescription} by operator {OperatorId}",
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
