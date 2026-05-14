// <copyright file="DataService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Ingest.Roles;

using Defra.Identity.KeeperReferenceData.Models.Parties;
using Defra.Identity.Repositories.Roles;

public class DataService(IRoleRepository repository) : IDataService<Role>
{
    public async Task Upsert(Defra.Identity.KeeperReferenceData.Models.Parties.Role role, CancellationToken cancellationToken = default)
    {
        var roles = await repository.GetSingle(x => x.Name.Equals(role.Code), cancellationToken);
        if (roles == null)
        {
            var newRole = MapIntegrationRoleToEntity(role);
            await repository.Create(newRole, cancellationToken);
        }
    }

    private static Postgres.Database.Entities.Roles MapIntegrationRoleToEntity(
        Role role)
    {
        return new Postgres.Database.Entities.Roles()
        {
            Name = role.Code ?? string.Empty,
            Description = role.Name ?? string.Empty,
        };
    }
}
