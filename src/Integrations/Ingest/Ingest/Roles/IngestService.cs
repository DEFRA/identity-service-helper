// <copyright file="IngestRoles.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using Defra.Identity.KeeperReferenceData.Models.Parties;
using Defra.Identity.KeeperReferenceData.Providers;
using Defra.Identity.Services.Roles;

namespace Defra.Identity.Ingest.Roles;

public class IngestService(IKrdsProvider provider, IRolesService service) : IIngestService<Postgres.Database.Entities.Roles>
{
    public async Task<bool> Execute()
    {
       var parties = await GetParties(DateTime.UtcNow);

       var partyRoles = parties.Values.SelectMany(x => x.PartyRoles).ToList();

       foreach (var role in partyRoles)
       {
           if (role.Role != null)
           {
               await service.Upsert(role.Role);
           }
       }

       return true;
    }

    private async Task<PartyResponse> GetParties(DateTime since)
    {
        return await provider.Parties(since, CancellationToken.None);
    }
}
