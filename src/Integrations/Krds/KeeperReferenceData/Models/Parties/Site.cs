// <copyright file="Site.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Defra.Identity.KeeperReferenceData.Models.Parties;

public class Site
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public object? Type { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("identifiers")]
    public List<Identifier> Identifiers { get; set; } = new();

    [JsonPropertyName("lastUpdatedDate")]
    public DateTimeOffset? LastUpdatedDate { get; set; }
}
