// <copyright file="CphAssignment.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Assignments;

using System.ComponentModel;

public class CphAssignment
{
    [Description(OpenApiMetadata.AssociatedUsers.Id)]
    public Guid Id { get; init; }

    [Description(OpenApiMetadata.Cphs.Id)]
    public required Guid CountyParishHoldingId { get; init; }

    [Description(OpenApiMetadata.Cphs.CphNumber)]
    public required string CountyParishHoldingNumber { get; set; }

    [Description(OpenApiMetadata.Users.Id)]
    public required Guid UserId { get; init; }

    [Description(OpenApiMetadata.Roles.Id)]
    public required Guid RoleId { get; init; }

    [Description(OpenApiMetadata.Roles.Name)]
    public required string RoleName { get; init; }

    [Description(OpenApiMetadata.Users.Email)]
    public required string Email { get; init; }

    [Description(OpenApiMetadata.Users.DisplayName)]
    public required string DisplayName { get; init; }

    [Description(OpenApiMetadata.AnimalSpecies.Id)]
    public string SpeciesTypeId { get; set; }

    [Description(OpenApiMetadata.AnimalSpecies.Name)]
    public string SpeciesTypeName { get; set; }
}
