// <copyright file="Communication.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models;

public class Communication
{
    public string Id { get; set; }

    public string Email { get; set; }

    public string Mobile { get; set; }

    public string Landline { get; set; }

    public bool PrimaryContactFlag { get; set; }

    public string LastUpdatedDate { get; set; }
}
