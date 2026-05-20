// <copyright file="UserWriteOperationBase.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Commands.Base;

using System.ComponentModel;

public abstract class UserWriteOperationBase
{
    [Description(OpenApiMetadata.Users.Email)]
    public string Email { get; init; } = string.Empty;

    [Description(OpenApiMetadata.Users.DisplayName)]
    public string DisplayName { get; init; } = string.Empty;

    [Description(OpenApiMetadata.Users.FirstName)]
    public string FirstName { get; init; } = string.Empty;

    [Description(OpenApiMetadata.Users.LastName)]
    public string LastName { get; init; } = string.Empty;
}
