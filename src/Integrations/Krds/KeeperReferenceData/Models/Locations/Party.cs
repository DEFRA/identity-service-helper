// <copyright file="Party.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models.Locations;

using System.Text.Json.Serialization;

public class Party
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("customerNumber")]
    public string CustomerNumber { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("partyType")]
    public string PartyType { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("lastUpdatedDate")]
    public DateTimeOffset? LastUpdatedDate { get; set; }

    [JsonPropertyName("communication")]
    public List<Communication> Communication { get; set; } = new();

    [JsonPropertyName("correspondanceAddress")]
    public Address? CorrespondanceAddress { get; set; }

    [JsonPropertyName("partyRoles")]
    public List<PartyRole> PartyRoles { get; set; } = new();
}
