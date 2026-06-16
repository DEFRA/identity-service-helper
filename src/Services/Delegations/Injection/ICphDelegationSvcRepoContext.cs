// <copyright file="ICphDelegationSvcRepoContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations.Injection;

using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Roles;
using Defra.Identity.Repositories.Users;

public interface ICphDelegationSvcRepoContext
{
    ICphDelegationsRepository DelegationRepository { get; }

    IUserRepository UserRepository { get; }

    ICphRepository CphRepository { get; }

    IRoleRepository RoleRepository { get; }
}
