// <copyright file="IDataService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Ingest;

public interface IDataService<in TModel>
{
    Task Upsert(TModel model, CancellationToken cancellationToken = default);
}
