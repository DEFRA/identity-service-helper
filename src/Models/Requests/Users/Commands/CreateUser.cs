// <copyright file="CreateUser.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Commands;

using System.ComponentModel;

public class CreateUser : User
{
    [Description(OpenApiMetadata.OperatorId)]
    public Guid OperatorId { get; set; } = Guid.Empty;

    [Description(OpenApiMetadata.Users.Email)]
    public string Email { get; set; } = string.Empty;
}
