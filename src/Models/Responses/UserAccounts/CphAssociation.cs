// <copyright file="CphAssociation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.UserAccounts;

using System.ComponentModel;

public class CphAssociation
{
    [Description(OpenApiMetadata.Generic.Id)]
    public required string Id { get; init; }

    [Description(OpenApiMetadata.Cphs.CphNumber)]
    public required string CphNumber { get; init; }

    [Description(OpenApiMetadata.UserAccounts.Role)]
    public required string Role { get; init; }

    [Description(OpenApiMetadata.UserAccounts.PartyId)]
    public string? PartyId { get; init; }

    [Description(OpenApiMetadata.UserAccounts.HoldingId)]
    public string? HoldingId { get; init; }

    [Description(OpenApiMetadata.UserAccounts.HoldingName)]
    public string? HoldingName { get; init; }
}
