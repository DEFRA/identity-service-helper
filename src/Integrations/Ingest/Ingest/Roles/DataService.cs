using Defra.Identity.KeeperReferenceData.Models.Parties;

namespace Defra.Identity.Ingest.Roles;

public class DataService : IDataService<Role>
{
    public async Task Upsert(Models.Integration.Krds.Parties.Role role, CancellationToken cancellationToken = default)
    {
        var roles = await repository.GetSingle(x => x.Name.Equals(role.Code), cancellationToken);
       

        var newRole = MapIntegrationRoleToEntity(role);
        await repository.Create(newRole, cancellationToken));
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