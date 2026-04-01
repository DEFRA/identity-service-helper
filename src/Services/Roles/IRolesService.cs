// <copyright file="IRolesService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Roles;

using Defra.Identity.Responses.Roles;

public interface IRolesService
{
    Task<Role> Upsert(Models.Integration.Krds.Parties.Role role, CancellationToken cancellationToken = default);
}
