// <copyright file="Country.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models;

public class Country
{
    public string Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public string LongName { get; set; }

    public bool EuTradeMemberFlag { get; set; }

    public bool DevolvedAuthorityFlag { get; set; }

    public string LastUpdatedDate { get; set; }
}
