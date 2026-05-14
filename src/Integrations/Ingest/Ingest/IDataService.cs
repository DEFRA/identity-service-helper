// <copyright file="IDataService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Ingest;

using System.Linq.Expressions;

public interface IDataService<TModel>
{
    Task Upsert(TModel model, CancellationToken cancellationToken = default);
}
