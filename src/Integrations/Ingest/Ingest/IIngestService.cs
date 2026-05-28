// <copyright file="IIngestService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Ingest;

#pragma warning disable S2326

public interface IIngestService<T>
    where T : class
{
    Task<bool> Execute();
}

#pragma warning restore S2326
