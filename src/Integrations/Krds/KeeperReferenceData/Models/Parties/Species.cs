// <copyright file="Species.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Defra.Identity.KeeperReferenceData.Models.Parties;

public class Species
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("startDate")]
    public DateTimeOffset? StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public DateTimeOffset? EndDate { get; set; }

    [JsonPropertyName("lastUpdatedDate")]
    public DateTimeOffset? LastUpdatedDate { get; set; }
}
