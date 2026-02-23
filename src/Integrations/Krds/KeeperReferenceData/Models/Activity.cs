// <copyright file="Activity.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models;

public class Activity
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string StartDate { get; set; }

    public object EndDate { get; set; }

    public string LastUpdatedDate { get; set; }
}
