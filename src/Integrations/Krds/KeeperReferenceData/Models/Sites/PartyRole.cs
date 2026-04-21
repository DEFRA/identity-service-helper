// <copyright file="PartyRole.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Defra.Identity.KeeperReferenceData.Models.Sites;

public class PartyRole
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("role")]
    public Role? Role { get; set; }

    [JsonPropertyName("speciesManagedByRole")]
    public List<Species> SpeciesManagedByRole { get; set; } = new();

    [JsonPropertyName("lastUpdatedDate")]
    public DateTimeOffset? LastUpdatedDate { get; set; }
}
