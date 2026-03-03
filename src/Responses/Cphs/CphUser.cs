// <copyright file="CphUser.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Responses.Cphs;

public class CphUser
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ApplicationId { get; set; }

    public Guid RoleId { get; set; }

    public required string Email { get; set; }

    public required string DisplayName { get; set; }
}
