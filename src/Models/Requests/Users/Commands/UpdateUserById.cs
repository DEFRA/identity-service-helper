// <copyright file="UpdateUserById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Commands;

using System.ComponentModel;

public class UpdateUserById : User
{
    [Description(OpenApiMetadata.Id)]
    public Guid Id { get; set; }
}
