// <copyright file="RolesService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Roles;

using Defra.Identity.Repositories.Roles;
using Defra.Identity.Responses.Roles;

public class RolesService(IRoleRepository repository) : IRolesService
{
    public async Task<Role> Upsert(Models.Integration.Krds.Parties.Role role, CancellationToken cancellationToken = default)
    {
        var roles = await repository.GetSingle(x => x.Name.Equals(role.Code), cancellationToken);
        if (roles != null)
        {
            return MapEntityToResponse(roles);
        }

        var newRole = MapIntegrationRoleToEntity(role);
        return MapEntityToResponse(await repository.Create(newRole, cancellationToken));
    }

    private static Postgres.Database.Entities.Roles MapIntegrationRoleToEntity(
        Models.Integration.Krds.Parties.Role role)
    {
        return new Postgres.Database.Entities.Roles()
        {
            Name = role.Code ?? string.Empty,
            Description = role.Name ?? string.Empty,
        };
    }

    private static Role MapEntityToResponse(Postgres.Database.Entities.Roles role)
    {
        return new Role
        {
            Description = role.Description,
            Name = role.Name,
            Id = role.Id,
        };
    }
}
