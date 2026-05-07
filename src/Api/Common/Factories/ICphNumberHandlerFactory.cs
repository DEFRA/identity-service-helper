// <copyright file="ICphNumberHandlerFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Common.Factories;

using Defra.Identity.Api.Middleware.Headers;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Cphs.Common;

public interface ICphNumberHandlerFactory<TService>
    where TService : class
{
    Func<QueryRequestHeaders, TSource, TService, Task<IResult>> CreateRerouteHandler<TTarget, TSource>(Func<QueryRequestHeaders, TTarget, TService, Task<IResult>> action)
        where TTarget : IOperationById, new()
        where TSource : IOperationByCphNumber;

    Func<CommandRequestHeaders, TSource, TService, Task<IResult>> CreateRerouteHandler<TTarget, TSource>(
        Func<CommandRequestHeaders, TTarget, TService, Task<IResult>> action)
        where TTarget : IOperationById, new()
        where TSource : IOperationByCphNumber;
}
