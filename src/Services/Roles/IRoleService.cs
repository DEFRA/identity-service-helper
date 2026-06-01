// <copyright file="IRoleService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Roles;

using Defra.Identity.Models.Responses.Roles;

public interface IRoleService
{
    Task<List<Role>> GetAll(CancellationToken cancellationToken = default);
}
