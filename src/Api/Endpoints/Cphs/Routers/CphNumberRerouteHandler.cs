// <copyright file="CphNumberRerouteHandler.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Cphs.Routers;

using Defra.Identity.Requests;
using Defra.Identity.Requests.Common;
using Defra.Identity.Requests.Common.Queries;
using Defra.Identity.Requests.Cphs;
using Defra.Identity.Services.Cphs;

public class CphNumberRerouteHandler<TTarget, TSource, THeaders>
    where TTarget : IOperationById, new()
    where TSource : IOperationByCphNumber
    where THeaders : class
{
    private readonly Func<THeaders, TTarget, ICphService, Task<IResult>> action;

    public CphNumberRerouteHandler(Func<THeaders, TTarget, ICphService, Task<IResult>> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        this.action = action;
    }

    public async Task<IResult> GetRerouteHandler(
        THeaders headers,
        [AsParameters] TSource sourceRequest,
        ICphService service)
    {
        var cphId = await service.GetIdFromCphNumber(sourceRequest.County, sourceRequest.Parish, sourceRequest.Holding);
        var targetRequest = CreateTargetRequestWithId(cphId);

        MapPagingQueryToTargetRequest(targetRequest, sourceRequest);

        return await this.action(headers, targetRequest, service);
    }

    private static TTarget CreateTargetRequestWithId(Guid id)
        => new()
        {
            Id = id,
        };

    private static void MapPagingQueryToTargetRequest(TTarget targetRequest, TSource sourceRequest)
    {
        if (sourceRequest is not PagedQuery mapFrom || targetRequest is not PagedQuery mapTo)
        {
            return;
        }

        mapTo.PageNumber = mapFrom.PageNumber;
        mapTo.PageSize = mapFrom.PageSize;
        mapTo.OrderByDescending = mapFrom.OrderByDescending;
    }
}
