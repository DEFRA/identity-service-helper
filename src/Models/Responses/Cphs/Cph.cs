// <copyright file="Cph.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Cphs;

using System.ComponentModel;
using Defra.Identity.Models.Responses.Species;

public class Cph
{
    [Description(OpenApiMetadata.Cphs.Id)]
    public required Guid Id { get; init; }

    [Description(OpenApiMetadata.Cphs.CphNumber)]
    public required string CountyParishHoldingNumber { get; init; }

    [Description(OpenApiMetadata.Generic.Expired)]
    public required bool Expired { get; init; }

    [Description(OpenApiMetadata.Generic.ExpiresAt)]
    public DateTime? ExpiredAt { get; init; }

    [Description(OpenApiMetadata.Cphs.AllowedSpecies)]
    public IEnumerable<AnimalSpecies> AllowedSpecies { get; init; } = [];
}
