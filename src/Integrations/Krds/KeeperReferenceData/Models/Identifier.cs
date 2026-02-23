// <copyright file="Identifier.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Models;

public class Identifier
{
    public string Id { get; set; }

    public string IdentifierName { get; set; }

    public string Type { get; set; }

    public string LastUpdatedDate { get; set; }
}
