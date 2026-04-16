// <copyright file="GetPagedStrategyBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Strategy;

using System.Linq.Expressions;
using Defra.Identity.Repositories.Common;
using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Requests.Common;
using Defra.Identity.Requests.Common.Queries;
using Defra.Identity.Responses.Common;
using Defra.Identity.Services.Common.Builders.Strategy.Base;
using Defra.Identity.Services.Common.Builders.Strategy.Constants;
using Microsoft.Extensions.Logging;

public class GetPagedStrategyBuilder<TService, TEntity> : StrategyBuilderBase<TService,
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

    public GetPagedStrategyBuilder<TService, TEntity> WithRequestAndEntityFilter<TRequest>(
        TRequest request,
        Expression<Func<TEntity, bool>> entityFilter)
        where TRequest : PagedQuery, IOperationById
    {
        Request = request;
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
            "Executing {ActionDescription} {EntityDescription}",
            ActionDescription.ToLowerInvariant(),
            PrimaryEntityDescription.ToLowerInvariant());

        await ExecuteRequestValidation();

        var pagedEntities = await PageableRepository.GetPaged(
            EntityFilter,
            Request.PageNumber,
            Request.PageSize,
            orderBy,
            Request.OrderByDescending ?? false,
            CancellationToken.Value);

        var associatedPagedResults = pagedEntities.ToPagedResults(map);

        Logger.LogInformation(
            "Successfully executed {ActionDescription} {EntityDescription}",
            ActionDescription.ToLowerInvariant(),
            PrimaryEntityDescription.ToLowerInvariant());

        return associatedPagedResults;
    }
}
