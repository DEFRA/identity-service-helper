// <copyright file="Site.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models.Sites;

using System.Text.Json.Serialization;

public class Site
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("lastUpdatedDate")]
    public DateTimeOffset? LastUpdatedDate { get; set; }

    [JsonPropertyName("type")]
    public object? Type { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("startDate")]
    public DateTimeOffset? StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public DateTimeOffset? EndDate { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("destroyIdentityDocumentsFlag")]
    public bool? DestroyIdentityDocumentsFlag { get; set; }

    [JsonPropertyName("location")]
    public Location? Location { get; set; }

    [JsonPropertyName("identifiers")]
    public List<Identifier> Identifiers { get; set; } = new();

    [JsonPropertyName("parties")]
    public List<Party> Parties { get; set; } = new();

    [JsonPropertyName("species")]
    public List<Species> Species { get; set; } = new();

    [JsonPropertyName("marks")]
    public List<Mark> Marks { get; set; } = new();

    [JsonPropertyName("activities")]
    public List<object> Activities { get; set; } = new();
}
