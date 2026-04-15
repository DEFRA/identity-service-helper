// <copyright file="UserAssociatedCph.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Responses.Users.Cphs;

public class UserAssociatedCph
{
    public required Guid AssociationId { get; set; }

    public required Guid CountyParishHoldingId { get; init; }

    public required string CountyParishHoldingNumber { get; set; }

    public required Guid ApplicationId { get; set; }

    public required Guid RoleId { get; set; }
}
