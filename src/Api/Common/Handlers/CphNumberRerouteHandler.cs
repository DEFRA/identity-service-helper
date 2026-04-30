// <copyright file="CphNumberRerouteHandler.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Common.Handlers;

using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Common.Queries;
using Defra.Identity.Models.Requests.Cphs.Common;
using Defra.Identity.Services.Cphs;

public class CphNumberRerouteHandler<TTarget, TSource, TService, THeaders>
    where TTarget : IOperationById, new()
    where TSource : IOperationByCphNumber
    where TService : class
    where THeaders : class
{
    private readonly ICphNumberService cphNumberService;
    private readonly Func<THeaders, TTarget, TService, Task<IResult>> action;

    public CphNumberRerouteHandler(ICphNumberService cphNumberService, Func<THeaders, TTarget, TService, Task<IResult>> action)
    {
        ArgumentNullException.ThrowIfNull(cphNumberService);
        ArgumentNullException.ThrowIfNull(action);

        this.cphNumberService = cphNumberService;
        this.action = action;
    }

    public async Task<IResult> Handler(
        THeaders headers,
        [AsParameters] TSource sourceRequest,
        TService service)
    {
        var cphId = await cphNumberService.GetIdFromCphNumber(sourceRequest);
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
