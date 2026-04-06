// <copyright file="UpdateStrategyBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Requests.Common;
using Defra.Identity.Services.Common.Builders.Rules;
using Defra.Identity.Services.Common.Builders.Strategy.Base;
using Defra.Identity.Services.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

public class UpdateStrategyBuilder<TService, TRepository, TEntity> : StrategyBuilderBase<TService, UpdateStrategyBuilder<TService, TRepository, TEntity>>
    where TService : class
    where TRepository : IGettable<TEntity>, IUpdatable<TEntity>
    where TEntity : class
{
    private TRepository? Repository { get; set; }

    private IOperationById? Request { get; set; }

    private Func<TEntity, Guid>? EntityIdRetriever { get; set; }

    private Action<TEntity>? UpdateAction { get; set; }

    private ExistenceRulesBuilder<TEntity>? ExistenceRulesBuilder { get; set; }

    private ReferenceRulesBuilder? ReferenceRulesBuilder { get; set; }

    private BusinessRulesBuilder<TEntity>? BusinessRulesBuilder { get; set; }

    public UpdateStrategyBuilder<TService, TRepository, TEntity> WithRepository(TRepository repository)
    {
        Repository = repository;
        return this;
    }

    public UpdateStrategyBuilder<TService, TRepository, TEntity> WithRequestToEntityMapping(IOperationById request, Func<TEntity, Guid>? entityIdRetriever)
    {
        Request = request;
        EntityIdRetriever = entityIdRetriever;
        return this;
    }

    public UpdateStrategyBuilder<TService, TRepository, TEntity> WithExistenceRules(Action<ExistenceRulesBuilder<TEntity>> builder)
    {
        ExistenceRulesBuilder = new ExistenceRulesBuilder<TEntity>();

        builder(ExistenceRulesBuilder);

        return this;
    }

    public UpdateStrategyBuilder<TService, TRepository, TEntity> WithReferenceRules(Action<ReferenceRulesBuilder> builder)
    {
        ReferenceRulesBuilder = new ReferenceRulesBuilder();

        builder(ReferenceRulesBuilder);

        return this;
    }

    public UpdateStrategyBuilder<TService, TRepository, TEntity> WithBusinessRules(Action<BusinessRulesBuilder<TEntity>> builder)
    {
        BusinessRulesBuilder = new BusinessRulesBuilder<TEntity>();

        builder(BusinessRulesBuilder);

        return this;
    }

    public UpdateStrategyBuilder<TService, TRepository, TEntity> WithUpdate(Action<TEntity> updateAction)
    {
        UpdateAction = updateAction;

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

        if (Request == null || EntityIdRetriever == null)
        {
            throw new InvalidOperationException("Request mapping must be provided for this operation");
        }

        if (UpdateAction == null)
        {
            throw new InvalidOperationException("Update action must be provided for this operation");
        }

        Logger.LogInformation(
            "Executing {ActionDescription} {EntityDescription} with id {Id} by operator {OperatorId}",
            ActionDescription.ToLowerInvariant(),
            EntityDescription.ToLowerInvariant(),
            Request.Id,
            OperatorContext.OperatorId);

        if (ValidateAction != null)
        {
            var validationResult = await ValidateAction();

            if (!validationResult.IsValid)
            {
                Logger.LogWarning(
                    "Execute {ActionDescription} {EntityDescription} with id {Id} failed basic validation",
                    ActionDescription.ToLowerInvariant(),
                    EntityDescription.ToLowerInvariant(),
                    Request.Id);

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
                        "Execute {ActionDescription} {EntityDescription} with id {Id} failed reference rule '{Description}'",
                        ActionDescription.ToLowerInvariant(),
                        EntityDescription.ToLowerInvariant(),
                        Request.Id,
                        rule.Description);

                    throw new NotFoundException(rule.Description);
                }
            }
        }

        var entityToUpdate = await Repository.GetSingle((entity) => Request.Id == EntityIdRetriever(entity), CancellationToken.Value);

        if (entityToUpdate == null)
        {
            Logger.LogWarning("{EntityDescription} with id {Id} not found", EntityDescription, Request.Id);

            throw new NotFoundException($"{EntityDescription} not found.");
        }

        if (ExistenceRulesBuilder != null)
        {
            foreach (var rule in ExistenceRulesBuilder.ExistenceRules)
            {
                var validAgainstExistenceRule = rule.Predicate(entityToUpdate);

                if (!validAgainstExistenceRule)
                {
                    Logger.LogWarning("{EntityDescription} with id {Id} not found", EntityDescription, Request.Id);

                    throw new NotFoundException($"{EntityDescription} not found.");
                }
            }
        }

        if (BusinessRulesBuilder != null)
        {
            foreach (var rule in BusinessRulesBuilder.BusinessRules)
            {
                var validAgainstBusinessRule = rule.Predicate(entityToUpdate);

                if (!validAgainstBusinessRule)
                {
                    Logger.LogWarning(
                        "Execute {ActionDescription} {EntityDescription} with id {Id} failed business rule '{Description}'",
                        ActionDescription.ToLowerInvariant(),
                        EntityDescription.ToLowerInvariant(),
                        Request.Id,
                        rule.Description);

                    throw new BusinessRuleException(rule.Description);
                }
            }
        }

        UpdateAction(entityToUpdate);

        var updatedEntity = await Repository.Update(entityToUpdate, CancellationToken.Value);

        Logger.LogInformation(
            "Successfully executed {ActionDescription} {EntityDescription} with id {Id} by operator {OperatorId}",
            ActionDescription.ToLowerInvariant(),
            EntityDescription.ToLowerInvariant(),
            Request.Id,
            OperatorContext.OperatorId);

        return updatedEntity;
    }

    public async Task<TResult> ExecuteAndMap<TResult>(Func<TEntity, TResult> map)
        where TResult : class
    {
        var entity = await Execute();

        return map(entity);
    }
}
