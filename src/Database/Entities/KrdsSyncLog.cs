// <copyright file="KrdsSyncLog.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Database.Entities;

using Livestock.Auth.Database.Entities.Base;

public class KrdsSyncLog : BaseProcessingEntity
{
    public required Guid CorrelationId { get; set; }

    public required string Upn { get; set; }

    public required string PayloadSha256 { get; set; }

    public required string SourceEndpoint { get; set; }

    public required int HttpStatus { get; set; }

    public required bool ProcessedOk { get; set; }

    public required string Message { get; set; }
}
