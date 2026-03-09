// <copyright file="Address.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models;

public class Address
{
    public string Id { get; set; } = string.Empty;

    public int Uprn { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;

    public string AddressLine2 { get; set; } = string.Empty;

    public string PostTown { get; set; } = string.Empty;

    public string County { get; set; } = string.Empty;

    public string Postcode { get; set; } = string.Empty;

    public Country Country { get; set; }

    public string LastUpdatedDate { get; set; } = string.Empty;
}
