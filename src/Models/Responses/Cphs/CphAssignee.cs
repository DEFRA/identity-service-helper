// <copyright file="CphAssignee.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Cphs;

using System.ComponentModel;
using Defra.Identity.Models;

public class CphAssignee
{
    [Description(OpenApiMetadata.AssociatedUsers.Id)]
    public Guid AssociationId { get; set; }

    [Description(OpenApiMetadata.Users.Id)]
    public Guid UserId { get; set; }

    [Description(OpenApiMetadata.Applications.Id)]
    public Guid ApplicationId { get; set; }

    [Description(OpenApiMetadata.Roles.Id)]
    public Guid RoleId { get; set; }

    [Description(OpenApiMetadata.Roles.Name)]
    public required string RoleName { get; init; }

    [Description(OpenApiMetadata.Users.Email)]
    public required string Email { get; set; }

    [Description(OpenApiMetadata.Users.DisplayName)]
    public required string DisplayName { get; set; }
}
