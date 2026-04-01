// <copyright file="Role.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Responses.Roles;

public class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}
