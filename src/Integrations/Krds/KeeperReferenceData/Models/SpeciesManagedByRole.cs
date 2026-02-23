// <copyright file="SpeciesManagedByRole.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models;

public class SpeciesManagedByRole
{
    public string Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public string StartDate { get; set; }

    public string EndDate { get; set; }

    public string LastUpdatedDate { get; set; }
}
