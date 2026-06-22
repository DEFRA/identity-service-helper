// <copyright file="CphDelegationSvcRepoContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations.Injection;

using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Roles;
using Defra.Identity.Repositories.Users;

public class CphDelegationSvcRepoContext : ICphDelegationSvcRepoContext
{
    public CphDelegationSvcRepoContext(
        ICphDelegationsRepository delegationRepository,
        IUserRepository userRepository,
        ICphRepository cphRepository,
        IRoleRepository roleRepository)
    {
        DelegationRepository = delegationRepository;
        UserRepository = userRepository;
        CphRepository = cphRepository;
        RoleRepository = roleRepository;
    }

    public ICphDelegationsRepository DelegationRepository { get; }

    public IUserRepository UserRepository { get; }

    public ICphRepository CphRepository { get; }

    public IRoleRepository RoleRepository { get; }
}
