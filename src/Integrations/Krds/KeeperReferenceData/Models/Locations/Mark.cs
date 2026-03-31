// <copyright file="Mark.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models.Locations;

using System.Text.Json.Serialization;

public class Mark
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("mark")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("startDate")]
    public DateTimeOffset? StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public DateTimeOffset? EndDate { get; set; }

    [JsonPropertyName("species")]
    public List<Species> Species { get; set; } = new();

    [JsonPropertyName("lastUpdatedDate")]
    public DateTimeOffset? LastUpdatedDate { get; set; }
}
