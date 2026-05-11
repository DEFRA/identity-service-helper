// <copyright file="User.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Users;

using System.ComponentModel;

public class User
{
    [Description(OpenApiMetadata.Users.Id)]
    public Guid Id { get; set; }

    [Description(OpenApiMetadata.Users.Email)]
    public string Email { get; set; } = string.Empty;

    [Description(OpenApiMetadata.Users.FirstName)]
    public string FirstName { get; set; } = string.Empty;

    [Description(OpenApiMetadata.Users.LastName)]
    public string LastName { get; set; } = string.Empty;

    [Description(OpenApiMetadata.Users.DisplayName)]
    public string DisplayName { get; set; } = string.Empty;
}
