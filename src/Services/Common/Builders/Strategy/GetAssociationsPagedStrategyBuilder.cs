// <copyright file="GetAssociationsPagedStrategyBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Common.Queries;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Repositories.Common;
using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Repositories.Common.Composites.Associations;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.Common.Builders.Rules;
using Defra.Identity.Services.Common.Builders.Strategy.Base;
using Defra.Identity.Services.Common.Builders.Strategy.Constants;
using Microsoft.Extensions.Logging;

public class GetAssociationsPagedStrategyBuilder<TService, TPrimary, TAssociation> : StrategyBuilderBase<TService,
    GetAssociationsPagedStrategyBuilder<TService, TPrimary, TAssociation>>
    where TService : class
    where TPrimary : class
    where TAssociation : class
{
    private IGettable<TPrimary>? GettablePrimaryRepository { get; set; }

    private IPageableAssociations<TPrimary, TAssociation>? PageableAssociationsRepository { get; set; }

    private IOperationById? RequestAsId { get; set; }

    private PagedQuery? RequestAsPagedQuery { get; set; }

    private Expression<Func<TPrimary, bool>>? PrimaryEntityFilter { get; set; }

    private Expression<Func<TAssociation, bool>>? AssociatedEntityFilter { get; set; }

    private ExistenceRulesBuilder<TService, TPrimary>? ExistenceRulesBuilder { get; set; }

    public GetAssociationsPagedStrategyBuilder<TService, TPrimary, TAssociation> WithPrimaryRepository<TRepository>(TRepository primaryRepository)
        where TRepository : IGettable<TPrimary>
    {
        GettablePrimaryRepository = primaryRepository;
        return this;
    }

    public GetAssociationsPagedStrategyBuilder<TService, TPrimary, TAssociation> WithAssociationsRepository<TRepository>(TRepository associationsRepository)
        where TRepository : IPageableAssociations<TPrimary, TAssociation>
    {
        PageableAssociationsRepository = associationsRepository;
        return this;
    }

    public GetAssociationsPagedStrategyBuilder<TService, TPrimary, TAssociation> WithRequestAndPrimaryEntityFilter<TRequest>(
        TRequest request,
        Expression<Func<TPrimary, bool>> primaryEntityFilter)
        where TRequest : PagedQuery, IOperationById
    {
        RequestAsId = request;
        RequestAsPagedQuery = request;
        PrimaryEntityFilter = primaryEntityFilter;
        return this;
    }

    public GetAssociationsPagedStrategyBuilder<TService, TPrimary, TAssociation> WithAssociatedEntityFilter(Expression<Func<TAssociation, bool>> associatedEntityFilter)
    {
        AssociatedEntityFilter = associatedEntityFilter;
        return this;
    }

    public GetAssociationsPagedStrategyBuilder<TService, TPrimary, TAssociation> WithPrimaryEntityExistenceRules(Action<ExistenceRulesBuilder<TService, TPrimary>> builder)
    {
        ExistenceRulesBuilder = new ExistenceRulesBuilder<TService, TPrimary>();

        builder(ExistenceRulesBuilder);

        return this;
    }

    public async Task<PagedResults<TResult>> ExecuteAndMap<TResult, TOrderBy>(Func<TAssociation, TResult> map, Expression<Func<TAssociation, TOrderBy>> orderBy)
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

        if (PageableAssociationsRepository == null)
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

        if (RequestAsId == null || RequestAsPagedQuery == null || PrimaryEntityFilter == null)
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
            RequestAsId.Id);

        await ExecuteRequestValidation();

        var primaryEntity = await GettablePrimaryRepository.GetSingle(PrimaryEntityFilter, CancellationToken.Value);

        if (primaryEntity == null)
        {
            Logger.LogWarning("{EntityDescription} with id {Id} not found", PrimaryEntityDescription, RequestAsId.Id);

            throw new NotFoundException($"{PrimaryEntityDescription} not found.");
        }

        ExistenceRulesBuilder?.Validate(RequestAsId, primaryEntity, PrimaryEntityDescription, Logger);

        var associatedPagedEntities = await PageableAssociationsRepository.GetPaged(
            PrimaryEntityFilter,
            AssociatedEntityFilter,
            RequestAsPagedQuery.PageNumber,
            RequestAsPagedQuery.PageSize,
            orderBy,
            RequestAsPagedQuery.OrderByDescending ?? false,
            CancellationToken.Value);

        var associatedPagedResults = associatedPagedEntities.ToPagedResults(map);

        Logger.LogInformation(
            "Successfully executed {ActionDescription} {EntityDescription} with id {Id}",
            ActionDescription.ToLowerInvariant(),
            PrimaryEntityDescription.ToLowerInvariant(),
            RequestAsId.Id);

        return associatedPagedResults;
    }
}
