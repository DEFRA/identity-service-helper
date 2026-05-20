// <copyright file="ICphNumberHandlerFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Common.Factories;

using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Cphs.Common;

public interface ICphNumberHandlerFactory<TService>
    where TService : class
{
    Func<TSource, TService, Task<IResult>> CreateRerouteHandler<TTarget, TSource>(Func<TTarget, TService, Task<IResult>> action)
        where TTarget : IOperationById<Guid>, new()
        where TSource : IOperationByCphNumber;
}
