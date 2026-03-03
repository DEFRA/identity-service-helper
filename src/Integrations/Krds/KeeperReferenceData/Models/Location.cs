// <copyright file="Location.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models;

public class Location
{
    public string Id { get; set; }

    public string OsMapReference { get; set; }

    public double Easting { get; set; }

    public double Northing { get; set; }

    public Address Address { get; set; }

    public Communication[] Communication { get; set; }

    public string LastUpdatedDate { get; set; }
}
