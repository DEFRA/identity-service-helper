// <copyright file="UpdateUser.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Commands;

using System.ComponentModel;

public class UpdateUser : User
{
    [Description(OpenApiMetadata.Users.Id)]
    public Guid Id { get; set; }

    [Description(OpenApiMetadata.Users.Email)]
    public string Email { get; set; } = string.Empty;

    [Description(OpenApiMetadata.Users.Id)]
    public Guid OperatorId { get; set; } = Guid.Empty;
}
