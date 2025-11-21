// <copyright file="KrdsSyncLog.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Database.Entities;

using Defra.Identity.Database.Entities.Base;

public class KrdsSyncLog : BaseProcessingEntity
{
    public Guid CorrelationId { get; set; }

    public required string SourceEndpoint { get; init; }

    public int HttpStatus { get; set; }

    public bool ProcessedOk { get; set; }

    public string Message { get; set; }
}
