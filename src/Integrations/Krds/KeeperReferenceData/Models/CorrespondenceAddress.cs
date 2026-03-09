// <copyright file="CorrespondenceAddress.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models;

public class CorrespondenceAddress
{
    public string Id { get; set; }

    public int Uprn { get; set; }

    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string PostTown { get; set; }

    public string County { get; set; }

    public string Postcode { get; set; }

    public Country Country { get; set; }

    public string LastUpdatedDate { get; set; }
}
