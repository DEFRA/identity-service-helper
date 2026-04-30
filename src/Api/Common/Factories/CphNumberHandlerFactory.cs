// <copyright file="CphHandlerFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Common.Factories;

using Defra.Identity.Api.Common.Handlers;
using Defra.Identity.Api.Middleware.Headers;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Cphs.Common;
using Defra.Identity.Services.Cphs;

public class CphNumberHandlerFactory<TService> : ICphNumberHandlerFactory<TService>
    where TService : class
{
    private readonly ICphNumberService cphNumberService;

    public CphNumberHandlerFactory(ICphNumberService cphNumberService)
    {
        this.cphNumberService = cphNumberService;
    }

    public Func<QueryRequestHeaders, TSource, TService, Task<IResult>> CreateRerouteHandler<TTarget, TSource>(Func<QueryRequestHeaders, TTarget, TService, Task<IResult>> action)
        where TTarget : IOperationById, new()
        where TSource : IOperationByCphNumber
    {
        return new CphNumberRerouteHandler<TTarget, TSource, TService, QueryRequestHeaders>(cphNumberService, action).Handler;
    }

    public Func<CommandRequestHeaders, TSource, TService, Task<IResult>> CreateRerouteHandler<TTarget, TSource>(
        Func<CommandRequestHeaders, TTarget, TService, Task<IResult>> action)
        where TTarget : IOperationById, new()
        where TSource : IOperationByCphNumber
    {
        return new CphNumberRerouteHandler<TTarget, TSource, TService, CommandRequestHeaders>(cphNumberService, action).Handler;
    }
}
