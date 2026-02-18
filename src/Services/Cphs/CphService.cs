// <copyright file="CphService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Requests.Cphs.Queries;
using Defra.Identity.Responses.Common;
using Defra.Identity.Responses.Cphs;
using Defra.Identity.Services.Extensions;
using Microsoft.Extensions.Logging;

public class CphService : ICphService
{
    private readonly ICphRepository repository;
    private readonly ILogger<CphService> logger;

    public CphService(ICphRepository repository, ILogger<CphService> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    public async Task<PagedResults<Cph>> GetAllPaged(GetCphs request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all county parish holdings by page");

        var includeExpired = IsExpiredInferred(request);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => (includeExpired || cph.ExpiredAt == null) && cph.DeletedAt == null;
        Expression<Func<CountyParishHoldings, string>> orderBy = cph => cph.Identifier;

        var pagedCphEntities = await repository.GetPaged(filter, request.PageNumber, request.PageSize, orderBy, request.OrderByDescending ?? false, cancellationToken);
        var pagedCphResults = pagedCphEntities.ToPagedResults(MapCphEntityToCph);

        return pagedCphResults;
    }

    public async Task<Cph> Get(GetCphById request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting county parish holding by id {Id}", request.Id);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Id == request.Id;

        var cphEntity = await repository.GetSingle(filter, cancellationToken);

        if (cphEntity is not { DeletedAt: null })
        {
            logger.LogWarning("County parish holding with id {Id} not found", request.Id);

            throw new NotFoundException("County parish holding not found.");
        }

        var cphResult = MapCphEntityToCph(cphEntity);

        return cphResult;
    }

    public async Task Expire(Guid id, Guid operatorId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Expiring county parish holding with id {Id} by operator {OperatorId}", id, operatorId);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Id == id;

        var cphEntity = await repository.GetSingle(filter, cancellationToken);

        if (cphEntity is not { DeletedAt: null })
        {
            logger.LogWarning("County parish holding with id {Id} not found", id);

            throw new NotFoundException("County parish holding not found.");
        }

        cphEntity.ExpiredAt = DateTime.UtcNow;

        await repository.Update(cphEntity, cancellationToken);
    }

    public async Task Delete(Guid id, Guid operatorId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting county parish holding with id {Id} by operator {OperatorId}", id, operatorId);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Id == id;

        var cphEntity = await repository.GetSingle(filter, cancellationToken);

        if (cphEntity is not { DeletedAt: null })
        {
            logger.LogWarning("County parish holding with id {Id} not found", id);

            throw new NotFoundException("County parish holding not found.");
        }

        cphEntity.DeletedById = operatorId;
        cphEntity.DeletedAt = DateTime.UtcNow;

        await repository.Update(cphEntity, cancellationToken);
    }

    private static Cph MapCphEntityToCph(CountyParishHoldings cphEntity)
        => new()
        {
            Id = cphEntity.Id, CphNumber = cphEntity.Identifier, Expired = cphEntity.ExpiredAt != null, ExpiredAt = cphEntity.ExpiredAt,
        };

    private static bool IsExpiredInferred(GetCphs request)
        => request.Expired != null && (request.Expired == string.Empty || request.Expired.Equals("true", StringComparison.InvariantCultureIgnoreCase));
}
