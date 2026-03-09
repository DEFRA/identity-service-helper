// <copyright file="PartyRole.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models;

public class PartyRole
{
    public string Id { get; set; }

    public Role Role { get; set; }

    public SpeciesManagedByRole[] SpeciesManagedByRole { get; set; }

    public string LastUpdatedDate { get; set; }
}
