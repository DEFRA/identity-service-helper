// <copyright file="DeleteUserById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Commands;

using System.ComponentModel;

public class DeleteUserById : User
{
    [Description(OpenApiMetadata.Id)]
    public Guid Id { get; set; }
}
