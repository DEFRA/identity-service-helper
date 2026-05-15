// <copyright file="GetPagedStrategyBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Common.Queries;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Repositories.Common;
using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Services.Common.Builders.Strategy.Base;
using Defra.Identity.Services.Common.Builders.Strategy.Constants;

public partial class GetPagedStrategyBuilder<TService, TEntity> : StrategyBuilderBase<TService,
    GetPagedStrategyBuilder<TService, TEntity>>
    where TService : class
    where TEntity : class
{
    private IPageable<TEntity>? PageableRepository { get; set; }

    private PagedQuery? Request { get; set; }

    private Expression<Func<TEntity, bool>>? EntityFilter { get; set; }

    public GetPagedStrategyBuilder<TService, TEntity> WithRepository<TRepository>(TRepository repository)
        where TRepository : IPageable<TEntity>
    {
        PageableRepository = repository;
        return this;
    }

    public GetPagedStrategyBuilder<TService, TEntity> WithRequest<TRequest>(TRequest request)
        where TRequest : PagedQuery
    {
        Request = request;
        return this;
    }

    public GetPagedStrategyBuilder<TService, TEntity> WithEntityFilter(Expression<Func<TEntity, bool>> entityFilter)
    {
        EntityFilter = entityFilter;
        return this;
    }

    public async Task<PagedResults<TResult>> ExecuteAndMap<TResult, TOrderBy>(Func<TEntity, TResult> map, Expression<Func<TEntity, TOrderBy>> orderBy)
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

        if (PageableRepository == null)
        {
            throw new InvalidOperationException(StrategyBuilderConstants.Errors.PageableRepositoryRequired);
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

        LogExecutingAction(Logger, ActionDescription.ToLowerInvariant(), EntityDescription.ToLowerInvariant());

        InvokeBeforeExecuteAction();

        await ExecuteRequestValidation();

        var pagedEntities = await PageableRepository.GetPaged(
            EntityFilter,
            Request.PageNumber,
            Request.PageSize,
            orderBy,
            Request.OrderByDescending ?? false,
            CancellationToken.Value);

        var associatedPagedResults = pagedEntities.ToPagedResults(map);

        LogSuccessfullyExecutedAction(Logger, ActionDescription.ToLowerInvariant(), EntityDescription.ToLowerInvariant());

        return associatedPagedResults;
    }
}
