// <copyright file="Activity.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models;

public class Activity
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string StartDate { get; set; } = string.Empty;

    public object EndDate { get; set; } = null!;

    public string LastUpdatedDate { get; set; } = string.Empty;
}
