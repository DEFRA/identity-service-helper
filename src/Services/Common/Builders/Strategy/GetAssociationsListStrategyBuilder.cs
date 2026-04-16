// <copyright file="GetAssociationsListStrategyBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using System.Linq.Expressions;
using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Repositories.Common.Composites.Associations;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Requests.Common;
using Defra.Identity.Services.Common.Builders.Rules;
using Defra.Identity.Services.Common.Builders.Strategy.Base;
using Defra.Identity.Services.Common.Builders.Strategy.Constants;
using Microsoft.Extensions.Logging;

public class GetAssociationsListStrategyBuilder<TService, TPrimary, TAssociation> : StrategyBuilderBase<TService,
    GetAssociationsListStrategyBuilder<TService, TPrimary, TAssociation>>
    where TService : class
    where TPrimary : class
    where TAssociation : class
{
    private IGettable<TPrimary>? GettablePrimaryRepository { get; set; }

    private IListableAssociations<TPrimary, TAssociation>? ListableAssociationsRepository { get; set; }

    private IOperationById? Request { get; set; }

    private Expression<Func<TPrimary, bool>>? PrimaryEntityFilter { get; set; }

    private Expression<Func<TAssociation, bool>>? AssociatedEntityFilter { get; set; }

    private ExistenceRulesBuilder<TPrimary>? ExistenceRulesBuilder { get; set; }

    public GetAssociationsListStrategyBuilder<TService, TPrimary, TAssociation> WithPrimaryRepository<TRepository>(TRepository primaryRepository)
        where TRepository : IGettable<TPrimary>
    {
        GettablePrimaryRepository = primaryRepository;
        return this;
    }

    public GetAssociationsListStrategyBuilder<TService, TPrimary, TAssociation> WithAssociationsRepository<TRepository>(TRepository associationsRepository)
        where TRepository : IListableAssociations<TPrimary, TAssociation>
    {
        ListableAssociationsRepository = associationsRepository;
        return this;
    }

    public GetAssociationsListStrategyBuilder<TService, TPrimary, TAssociation> WithRequestAndPrimaryEntityFilter(
        IOperationById request,
        Expression<Func<TPrimary, bool>> primaryEntityFilter)
    {
        Request = request;
        PrimaryEntityFilter = primaryEntityFilter;
        return this;
    }

    public GetAssociationsListStrategyBuilder<TService, TPrimary, TAssociation> WithAssociatedEntityFilter(Expression<Func<TAssociation, bool>> associatedEntityFilter)
    {
        AssociatedEntityFilter = associatedEntityFilter;
        return this;
    }

    public GetAssociationsListStrategyBuilder<TService, TPrimary, TAssociation> WithPrimaryEntityExistenceRules(Action<ExistenceRulesBuilder<TPrimary>> builder)
    {
        ExistenceRulesBuilder = new ExistenceRulesBuilder<TPrimary>();

        builder(ExistenceRulesBuilder);

        return this;
    }

    public async Task<List<TResult>> ExecuteAndMap<TResult>(Func<TAssociation, TResult> map)
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

        if (GettablePrimaryRepository == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.PrimaryRepositoryRequired);
        }

        if (ListableAssociationsRepository == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.AssociationsRepositoryRequired);
        }

        if (PrimaryEntityDescription == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.PrimaryEntityDescriptionRequired);
        }

        if (ActionDescription == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.ActionDescriptionRequired);
        }

        if (Request == null || PrimaryEntityFilter == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.RequestAndEntityFilterRequired);
        }

        if (AssociatedEntityFilter == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.AssociatedEntityFilterRequired);
        }

        Logger.LogInformation(
            "Executing {ActionDescription} {EntityDescription} with id {Id}",
            ActionDescription.ToLowerInvariant(),
            PrimaryEntityDescription.ToLowerInvariant(),
            Request.Id);

        await ExecuteRequestValidation();

        var primaryEntity = await GettablePrimaryRepository.GetSingle(PrimaryEntityFilter, CancellationToken.Value);

        if (primaryEntity == null)
        {
            Logger.LogWarning("{EntityDescription} with id {Id} not found", PrimaryEntityDescription, Request.Id);

            throw new NotFoundException($"{PrimaryEntityDescription} not found.");
        }

        if (ExistenceRulesBuilder != null)
        {
            foreach (var rule in ExistenceRulesBuilder.ExistenceRules)
            {
                var validAgainstExistenceRule = rule.Predicate(primaryEntity);

                if (!validAgainstExistenceRule)
                {
                    Logger.LogWarning("{EntityDescription} with id {Id} not found", PrimaryEntityDescription, Request.Id);

                    throw new NotFoundException($"{PrimaryEntityDescription} not found.");
                }
            }
        }

        var associatedEntities = await ListableAssociationsRepository.GetList(PrimaryEntityFilter, AssociatedEntityFilter, CancellationToken.Value);

        var mappedEntities = associatedEntities.Select(map).ToList();

        Logger.LogInformation(
            "Successfully executed {ActionDescription} {EntityDescription} with id {Id}",
            ActionDescription.ToLowerInvariant(),
            PrimaryEntityDescription.ToLowerInvariant(),
            Request.Id);

        return mappedEntities;
    }
}
