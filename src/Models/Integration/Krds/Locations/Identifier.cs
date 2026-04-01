// <copyright file="Identifier.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Integration.Krds.Locations;

using System.Text.Json.Serialization;

public class Identifier
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("identifier")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public Type? Type { get; set; }

    [JsonPropertyName("lastUpdatedDate")]
    public DateTimeOffset? LastUpdatedDate { get; set; }
}
