// <copyright file="RoleMapper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Mappers;

using Defra.Identity.Models.Responses.Roles;
using Defra.Identity.Postgres.Database.Entities;

public static class RoleMapper
{
    public static Role MapRoleEntityToRole(Roles roleEntity)
    {
        return new Role()
        {
            Id = roleEntity.Id, Name = roleEntity.Name, Description = roleEntity.Description,
        };
    }
}
