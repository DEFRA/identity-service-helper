// <copyright file="Mark.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models;

public class Mark
{
    public string Id { get; set; }

    public string MarkValue { get; set; }

    public Species Species { get; set; }

    public string StartDate { get; set; }

    public object EndDate { get; set; }
}
