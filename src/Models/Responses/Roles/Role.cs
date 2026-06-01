// <copyright file="Role.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Roles;

using System.ComponentModel;

public class Role
{
    [Description(OpenApiMetadata.Roles.Id)]
    public Guid Id { get; set; }

    [Description(OpenApiMetadata.Roles.Name)]
    public required string Name { get; set; }

    [Description(OpenApiMetadata.Roles.Description)]
    public required string Description { get; set; }
}
