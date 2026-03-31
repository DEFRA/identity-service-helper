// <copyright file="IIngestDataService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Ingest;

public interface IIngestDataService
{
    Task<bool> Execute();
}
