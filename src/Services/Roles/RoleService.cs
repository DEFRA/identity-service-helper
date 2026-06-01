// <copyright file="RoleService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Roles;

using Defra.Identity.Models.Responses.Roles;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Roles;
using Defra.Identity.Services.Common;
using Defra.Identity.Services.Common.Builders.Strategy.Factories;
using Defra.Identity.Services.Common.Filters;
using Defra.Identity.Services.Common.Mappers;
using Microsoft.Extensions.Logging;

public class RoleService : IRoleService
{
    private readonly IRoleRepository repository;
    private readonly IStrategyBuilderFactory<RoleService> strategyBuilderFactory;

    public RoleService(
        IRoleRepository repository,
        IStrategyBuilderFactory<RoleService> strategyBuilderFactory,
        ILogger<RoleService> logger)
    {
        this.repository = repository;
        this.strategyBuilderFactory = strategyBuilderFactory;

        this.strategyBuilderFactory
            .WithDefaultLogger(logger)
            .WithDefaultEntityDescription(EntityDescriptions.Role);
    }

    public async Task<List<Role>> GetAll(CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildGetListStrategy<Roles>()
            .WithActionDescription("Get all roles")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithEntityFilter(FilterLibrary.Roles.All)
            .ExecuteAndMap(RoleMapper.MapRoleEntityToRole);
    }
}
