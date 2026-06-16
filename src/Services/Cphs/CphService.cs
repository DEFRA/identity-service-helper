// <copyright file="CphService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs;

using Defra.Identity.Models.Requests.Common.Queries;
using Defra.Identity.Models.Requests.Cphs.Commands;
using Defra.Identity.Models.Requests.Cphs.Queries;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Models.Responses.Cphs;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Services.Common;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Common.Filters;
using Defra.Identity.Services.Common.Mappers;
using Defra.Identity.Services.Common.Strategy.Factories;
using Defra.Identity.Services.Cphs.Rules;
using FluentValidation;
using Microsoft.Extensions.Logging;

public class CphService : ICphService
{
    private readonly ICphRepository repository;
    private readonly IOperatorContext operatorContext;
    private readonly IStrategyBuilderFactory<CphService> strategyBuilderFactory;
    private readonly IValidator<PagedQuery> pagedQueryValidator;

    public CphService(
        ICphRepository repository,
        IOperatorContext operatorContext,
        IStrategyBuilderFactory<CphService> strategyBuilderFactory,
        IValidator<PagedQuery> pagedQueryValidator,
        ILogger<CphService> logger)
    {
        this.repository = repository;
        this.operatorContext = operatorContext;
        this.strategyBuilderFactory = strategyBuilderFactory;
        this.pagedQueryValidator = pagedQueryValidator;

        this.strategyBuilderFactory
            .WithDefaultLogger(logger)
            .WithDefaultOperatorContext(this.operatorContext)
            .WithDefaultEntityDescription(EntityDescriptions.Cph);
    }

    public async Task<PagedResults<Cph>> GetAllPaged(GetCphs request, CancellationToken cancellationToken = default)
    {
        var cphFilter = IncludeExpiredInferred(request) ? FilterLibrary.Cphs.NotSoftDeleted : FilterLibrary.Cphs.NotSoftDeletedOrExpired;

        return await strategyBuilderFactory.BuildGetPagedStrategy<CountyParishHoldings>()
            .WithActionDescription("Get all county parish holdings paged")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithRequestValidation(() => pagedQueryValidator.ValidateAsync(request, cancellationToken))
            .WithEntityFilter(cphFilter)
            .ExecuteAndMap(CphMapper.MapCphEntityToCph, cph => cph.Identifier);
    }

    public async Task<Cph> Get(GetCphByCphId request, CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildGetStrategy<CountyParishHoldings>()
            .WithActionDescription("Get county parish holding")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(cph => request.Id == cph.Id)
            .WithExistenceRules(rules => rules.Add(RulesLibrary.Existence.NotSoftDeleted))
            .ExecuteAndMap(CphMapper.MapCphEntityToCph);
    }

    public async Task Expire(ExpireCphByCphId request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy<CountyParishHoldings>()
            .WithActionDescription("Expire county parish holding")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(cph => request.Id == cph.Id)
            .WithExistenceRules(rules => rules.Add(RulesLibrary.Existence.NotSoftDeleted))
            .WithConflictRules(rules => rules.Add(RulesLibrary.Conflict.NotAlreadyExpired))
            .WithUpdate(cph => { cph.ExpiredAt = DateTime.UtcNow; })
            .Execute();
    }

    public async Task Delete(DeleteCphByCphId request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy<CountyParishHoldings>()
            .WithActionDescription("Delete county parish holding")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(cph => request.Id == cph.Id)
            .WithExistenceRules(rules => rules.Add(RulesLibrary.Existence.NotSoftDeleted))
            .WithUpdate(
                cph =>
                {
                    cph.DeletedAt = DateTime.UtcNow;
                    cph.DeletedById = operatorContext.OperatorId;
                })
            .Execute();
    }

    private static bool IncludeExpiredInferred(GetCphs request)
    {
        return request.Expired != null &&
               (request.Expired == string.Empty ||
                request.Expired.Equals("true", StringComparison.InvariantCultureIgnoreCase));
    }
}
