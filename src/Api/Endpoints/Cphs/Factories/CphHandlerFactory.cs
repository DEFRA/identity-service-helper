// <copyright file="CphHandlerFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Cphs.Factories;

using Defra.Identity.Api.Endpoints.Cphs.Handlers;
using Defra.Identity.Api.Middleware.Headers;
using Defra.Identity.Models.Requests;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Cphs.Common;
using Defra.Identity.Services.Cphs;

public static class CphHandlerFactory
{
    public static Func<QueryRequestHeaders, TSource, ICphService, Task<IResult>> CreateCphNumberRerouteHandler<TTarget, TSource>(
        Func<QueryRequestHeaders, TTarget, ICphService, Task<IResult>> action)
        where TTarget : IOperationById, new()
        where TSource : IOperationByCphNumber
    {
        return new CphNumberRerouteHandler<TTarget, TSource, QueryRequestHeaders>(action).Handler;
    }

    public static Func<CommandRequestHeaders, TSource, ICphService, Task<IResult>> CreateCphNumberRerouteHandler<TTarget, TSource>(
        Func<CommandRequestHeaders, TTarget, ICphService, Task<IResult>> action)
        where TTarget : IOperationById, new()
        where TSource : IOperationByCphNumber
    {
        return new CphNumberRerouteHandler<TTarget, TSource, CommandRequestHeaders>(action).Handler;
    }
}
