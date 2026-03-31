// <copyright file="Party.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models.Parties;

using System.Text.Json.Serialization;

public class Party
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("lastUpdatedDate")]
    public DateTimeOffset? LastUpdatedDate { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("customerNumber")]
    public string? CustomerNumber { get; set; }

    [JsonPropertyName("partyType")]
    public string? PartyType { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("communication")]
    public List<Communication> Communication { get; set; } = new();

    [JsonPropertyName("correspondanceAddress")]
    public Address? CorrespondanceAddress { get; set; }

    [JsonPropertyName("partyRoles")]
    public List<PartyRole> PartyRoles { get; set; } = new();
}
