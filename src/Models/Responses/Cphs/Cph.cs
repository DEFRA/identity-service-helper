// <copyright file="Cph.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Cphs;

using System.ComponentModel;

public class Cph
{
    [Description(OpenApiMetadata.Cphs.Id)]
    public Guid Id { get; set; }

    [Description(OpenApiMetadata.Cphs.CphNumber)]
    public required string CountyParishHoldingNumber { get; set; }

    [Description(OpenApiMetadata.Expired)]
    public bool Expired { get; set; }

    [Description(OpenApiMetadata.ExpiresAt)]
    public DateTime? ExpiredAt { get; set; }
}
