// <copyright file="CphAssociation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models.UserAccounts;

using System.Text.Json.Serialization;

public class CphAssociation
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("cphNumber")]
    public string CphNumber { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("partyId")]
    public string? PartyId { get; set; }

    [JsonPropertyName("holdingId")]
    public string? HoldingId { get; set; }

    [JsonPropertyName("holdingName")]
    public string? HoldingName { get; set; }
}
