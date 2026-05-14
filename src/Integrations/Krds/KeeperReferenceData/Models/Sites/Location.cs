// <copyright file="Location.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models.Sites;

using System.Text.Json.Serialization;

public class Location
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("osMapReference")]
    public string? OsMapReference { get; set; }

    [JsonPropertyName("easting")]
    public double? Easting { get; set; }

    [JsonPropertyName("northing")]
    public double? Northing { get; set; }

    [JsonPropertyName("address")]
    public Address? Address { get; set; }

    [JsonPropertyName("communication")]
    public List<Communication> Communication { get; set; } = new();

    [JsonPropertyName("lastUpdatedDate")]
    public DateTimeOffset? LastUpdatedDate { get; set; }
}
