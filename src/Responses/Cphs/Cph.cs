// <copyright file="Cph.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Responses.Cphs;

public class Cph
{
    public Guid Id { get; set; }

    public required string CphNumber { get; set; }

    public bool Expired { get; set; }

    public DateTime? ExpiredAt { get; set; }
}
