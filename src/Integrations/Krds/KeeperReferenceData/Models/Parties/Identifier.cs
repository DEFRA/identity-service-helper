// <copyright file="Identifier.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Defra.Identity.KeeperReferenceData.Models.Parties;

public class Identifier
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("identifier")]
    public string? Value { get; set; }

    [JsonPropertyName("type")]
    public IdentifierType? Type { get; set; }

    [JsonPropertyName("lastUpdatedDate")]
    public DateTimeOffset? LastUpdatedDate { get; set; }
}
