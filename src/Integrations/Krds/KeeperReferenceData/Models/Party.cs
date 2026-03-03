// <copyright file="Party.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models;

public class Party
{
    public string Id { get; set; }

    public string Title { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Name { get; set; }

    public string CustomerNumber { get; set; }

    public string PartyType { get; set; }

    public Communication[] Communication { get; set; }

    public CorrespondenceAddress CorrespondenceAddress { get; set; }

    public PartyRole[] PartyRoles { get; set; }

    public string State { get; set; }

    public string LastUpdatedDate { get; set; }
}
