// <copyright file="IngestRoles.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Ingest;

using Defra.Identity.KeeperReferenceData.Providers;
using Defra.Identity.Models.Integration.Krds.Parties;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Roles;

public class IngestRoles(IKrdsProvider provider, IRolesService service) : IIngestDataService<Roles>
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
