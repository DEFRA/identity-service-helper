// <copyright file="Communication.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Integration.Krds.Locations;

using System.Text.Json.Serialization;

public class Communication
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    [JsonPropertyName("landline")]
    public string? Landline { get; set; }

    [JsonPropertyName("primaryContactFlag")]
    public bool PrimaryContactFlag { get; set; }

    [JsonPropertyName("lastUpdatedDate")]
    public DateTimeOffset? LastUpdatedDate { get; set; }
}
