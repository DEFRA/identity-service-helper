// <copyright file="Applications.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using Defra.Identity.Extensions;

namespace Defra.Identity.Mongo.Database.Documents;

using Defra.Identity.Mongo.Database.Attributes;
using MongoDB.Bson.Serialization.Attributes;

[CollectionName(nameof(Applications))]
public class Applications : BaseAudit
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public required string Description { get; set; } = string.Empty;

    [BsonElement("client_id")]
    public required Guid ClientId { get; set; } = Guid.Empty;

    [BsonElement("tenant_name")]
    public required string TenantName { get; set; } = string.Empty;

    [BsonElement("status")]
    [MaxLength(30, ErrorMessage = "Status cannot exceed 30 characters")]
    public required string Status { get; set; } = string.Empty;
}
