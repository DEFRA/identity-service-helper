// <copyright file="CphNumberHandlerFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Common.Factories;

using Defra.Identity.Api.Common.Handlers;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Cphs.Common;
using Defra.Identity.Services.Cphs;

public class CphNumberHandlerFactory<TService>(ICphNumberService cphNumberService)
    : ICphNumberHandlerFactory<TService>
    where TService : class
{
    public Func<TSource, TService, Task<IResult>> CreateRerouteHandler<TTarget, TSource>(Func<TTarget, TService, Task<IResult>> action)
        where TTarget : IOperationById<Guid>, new()
        where TSource : IOperationByCphNumber
    {
        return new CphNumberRerouteHandler<TTarget, TSource, TService>(cphNumberService, action).Handler;
    }
}
