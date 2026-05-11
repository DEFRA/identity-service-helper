// <copyright file="CphService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs;

using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Cphs.Commands;
using Defra.Identity.Models.Requests.Cphs.Queries;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Models.Responses.Cphs;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Services.Common.Exceptions;
using Microsoft.Extensions.Logging;

public partial class CphService(
    ICphRepository cphRepository,
    ILogger<CphService> logger)
    : ICphService
{
    public async Task<PagedResults<Cph>> GetAllPaged(GetCphs request, CancellationToken cancellationToken = default)
    {
        LogGettingAllCountyParishHoldingsByPage();

        var includeExpired = IsExpiredInferred(request);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => (includeExpired || cph.ExpiredAt == null) && cph.DeletedAt == null;
        Expression<Func<CountyParishHoldings, string>> orderBy = cph => cph.Identifier;

        var pagedCphEntities = await cphRepository.GetPaged(filter, request.PageNumber, request.PageSize, orderBy, request.OrderByDescending ?? false, cancellationToken);
        var pagedCphResults = pagedCphEntities.ToPagedResults(MapCphEntityToCph);

        return pagedCphResults;
    }

    public async Task<Cph> Get(GetCphByCphId request, CancellationToken cancellationToken = default)
    {
        LogGettingCountyParishHoldingById(request.Id);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Id == request.Id;

        var cphEntity = await cphRepository.GetSingle(filter, cancellationToken);

        if (cphEntity is not { DeletedAt: null })
        {
            LogCountyParishHoldingWithIdNotFound(request.Id);

            throw new NotFoundException("County parish holding not found.");
        }

        var cphResult = MapCphEntityToCph(cphEntity);

        return cphResult;
    }

    public async Task Expire(ExpireCphByCphId request, Guid operatorId, CancellationToken cancellationToken = default)
    {
        LogExpiringCountyParishHoldingWithIdByOperatorid(request.Id, operatorId);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Id == request.Id;

        var cphEntity = await cphRepository.GetSingle(filter, cancellationToken);

        if (cphEntity is not { DeletedAt: null })
        {
            LogCountyParishHoldingWithIdNotFound(request.Id);

            throw new NotFoundException("County parish holding not found.");
        }

        if (cphEntity.ExpiredAt != null)
        {
            LogCountyParishHoldingWithIdIsAlreadyExpired(request.Id);

            throw new ConflictException("County parish holding already expired.");
        }

        cphEntity.ExpiredAt = DateTime.UtcNow;

        await cphRepository.Update(cphEntity, cancellationToken);
    }

    public async Task Delete(DeleteCphByCphId request, Guid operatorId, CancellationToken cancellationToken = default)
    {
        LogDeletingCountyParishHoldingWithIdByOperatorid(request.Id, operatorId);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Id == request.Id;

        var cphEntity = await cphRepository.GetSingle(filter, cancellationToken);

        if (cphEntity is not { DeletedAt: null })
        {
            LogCountyParishHoldingWithIdNotFound(request.Id);

            throw new NotFoundException("County parish holding not found.");
        }

        cphEntity.DeletedById = operatorId;
        cphEntity.DeletedAt = DateTime.UtcNow;

        await cphRepository.Update(cphEntity, cancellationToken);
    }

    private static Cph MapCphEntityToCph(CountyParishHoldings cphEntity)
    {
        return new Cph
        {
            Id = cphEntity.Id, CountyParishHoldingNumber = cphEntity.Identifier, Expired = cphEntity.ExpiredAt != null, ExpiredAt = cphEntity.ExpiredAt,
        };
    }

    private static bool IsExpiredInferred(GetCphs request)
    {
        return request.Expired != null && (request.Expired == string.Empty || request.Expired.Equals("true", StringComparison.InvariantCultureIgnoreCase));
    }
}
