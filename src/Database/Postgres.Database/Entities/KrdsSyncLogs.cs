// <copyright file="KrdsSyncLogs.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Entities;

using Defra.Identity.Postgres.Database.Entities.Base;

public class KrdsSyncLogs : BaseProcessingEntity
{
    public Guid CorrelationId { get; set; }

    public required string SourceEndpoint { get; init; }

    public int HttpStatus { get; set; }

    public bool ProcessedOk { get; set; }

    public string Message { get; set; } = string.Empty;
}
