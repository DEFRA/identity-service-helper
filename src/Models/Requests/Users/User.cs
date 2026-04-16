// <copyright file="User.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users;

using System.ComponentModel;

public abstract class User
{
    [Description(OpenApiMetadata.Users.DisplayName)]
    public string DisplayName { get; set; } = string.Empty;

    [Description(OpenApiMetadata.Users.FirstName)]
    public string FirstName { get; set; } = string.Empty;

    [Description(OpenApiMetadata.Users.LastName)]
    public string LastName { get; set; } = string.Empty;
}
