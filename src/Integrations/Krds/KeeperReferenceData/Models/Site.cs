// <copyright file="Site.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models;

public class Site
{
    public string Id { get; set; }

    public Type Type { get; set; }

    public string Name { get; set; }

    public Identifier[] Identifiers { get; set; }

    public string State { get; set; }

    public string LastUpdatedDate { get; set; }

    public string StartDate { get; set; }

    public object EndDate { get; set; }

    public string Source { get; set; }

    public bool DestroyIdentityDocumentsFlag { get; set; }

    public Location Location { get; set; }

    public Party[] Parties { get; set; }

    public Species[] Species { get; set; }

    public Mark[] Marks { get; set; }

    public Activity[] Activities { get; set; }
}
