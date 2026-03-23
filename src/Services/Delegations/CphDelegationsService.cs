// <copyright file="CphDelegationsService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Repositories.Roles;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Requests.Delegations.Commands.Common;
using Defra.Identity.Requests.Delegations.Commands.Create;
using Defra.Identity.Requests.Delegations.Commands.Update;
using Defra.Identity.Requests.Delegations.Queries;
using Defra.Identity.Responses.Delegations;
using Microsoft.Extensions.Logging;

public class CphDelegationsService : ICphDelegationsService
{
    private readonly ICphDelegationsRepository repository;
    private readonly IUsersRepository usersRepository;
    private readonly ICphRepository cphRepository;
    private readonly IRoleRepository roleRepository;
    private readonly ILogger<CphDelegationsService> logger;

    public CphDelegationsService(
        ICphDelegationsRepository repository,
        IUsersRepository usersRepository,
        ICphRepository cphRepository,
        IRoleRepository roleRepository,
        ILogger<CphDelegationsService> logger)
    {
        this.repository = repository;
        this.usersRepository = usersRepository;
        this.cphRepository = cphRepository;
        this.roleRepository = roleRepository;
        this.logger = logger;
    }

    public async Task<List<CphDelegation>> GetAll(GetCphDelegations request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all delegations");
        var entities = await repository.GetList(x => true, cancellationToken);

        return entities.Select(MapToResponse).ToList();
    }

    public async Task<CphDelegation> Get(GetCphDelegationById request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting delegation by id {Id}", request.Id);
        Expression<Func<CountyParishHoldingDelegations, bool>> filter = x => x.Id == request.Id;

        var entity = await repository.GetSingle(filter, cancellationToken);
        if (entity == null)
        {
            logger.LogWarning("Delegation with id {Id} not found", request.Id);
            throw new NotFoundException("Delegation not found.");
        }

        return MapToResponse(entity);
    }

    public async Task<CphDelegation> Update(UpdateCphDelegationById request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating delegation with id {Id}", request.Id);

        var existing = await repository.GetSingle(x => x.Id.Equals(request.Id), cancellationToken);
        if (existing == null)
        {
            logger.LogWarning("Delegation with id {Id} not found for update", request.Id);
            throw new NotFoundException($"Delegation with id {request.Id} not found.");
        }

        await ValidateReferences(request, cancellationToken);

        existing.CountyParishHoldingId = request.CountyParishHoldingId;
        existing.DelegatingUserId = request.DelegatingUserId;
        existing.DelegatedUserId = request.DelegatedUserId;
        existing.DelegatedUserEmail = request.DelegatedUserEmail;
        existing.DelegatedUserRoleId = request.DelegatedUserRoleId;

        var updated = await repository.Update(existing, cancellationToken);
        return MapToResponse(updated);
    }

    public async Task<CphDelegation> Create(CreateCphDelegation request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new delegation for county parish holding {CountParishHoldingId} and user {UserId}", request.CountyParishHoldingId, request.DelegatedUserId);

        await ValidateReferences(request, cancellationToken);

        var entity = new CountyParishHoldingDelegations
        {
            CountyParishHoldingId = request.CountyParishHoldingId,
            DelegatingUserId = request.DelegatingUserId,
            DelegatedUserId = request.DelegatedUserId,
            DelegatedUserEmail = request.DelegatedUserEmail,
            DelegatedUserRoleId = request.DelegatedUserRoleId,
            InvitationToken = string.Empty,
            InvitationExpiresAt = DateTime.Now.AddDays(2).ToUniversalTime(),
            CreatedById = request.OperatorId,
        };

        var created = await repository.Create(entity, cancellationToken);

        return MapToResponse(created);
    }

    public async Task<bool> Delete(Guid id, Guid operatorId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting delegation with id {Id} by operator {OperatorId}", id, operatorId);
        return await repository.Delete(x => x.Id == id, operatorId, cancellationToken);
    }

    private async Task ValidateReferences(CphDelegationWriteOperation request, CancellationToken cancellationToken)
    {
        var countyParishHolding = await cphRepository.GetSingle(x => x.Id == request.CountyParishHoldingId, cancellationToken);

        if (countyParishHolding == null)
        {
            logger.LogWarning("CountyParishHolding with id {Id} not found", request.CountyParishHoldingId);
            throw new NotFoundException($"CountyParishHolding with id {request.CountyParishHoldingId} not found.");
        }

        var delegatingUser = await usersRepository.GetSingle(x => x.Id == request.DelegatingUserId, cancellationToken);

        if (delegatingUser == null)
        {
            logger.LogWarning("User with id {Id} not found", request.DelegatingUserId);
            throw new NotFoundException($"User with id {request.DelegatingUserId} not found.");
        }

        var delegatedRole = await roleRepository.GetSingle(x => x.Id == request.DelegatedUserRoleId, cancellationToken);

        if (delegatedRole == null)
        {
            logger.LogWarning("Role with id {Id} not found", request.DelegatedUserRoleId);
            throw new NotFoundException($"Role with id {request.DelegatedUserRoleId} not found.");
        }

        if (request.DelegatedUserId is not null)
        {
            delegatingUser = await usersRepository.GetSingle(x => x.Id == request.DelegatedUserId, cancellationToken);

            if (delegatingUser == null)
            {
                logger.LogWarning("User with id {Id} not found", request.DelegatedUserId);
                throw new NotFoundException($"User with id {request.DelegatedUserId} not found.");
            }
        }
    }

    private static CphDelegation MapToResponse(CountyParishHoldingDelegations entity)
    {
        return new CphDelegation
        {
            Id = entity.Id,
            CountyParishHoldingId = entity.CountyParishHoldingId,
            CountyParishHoldingNumber = entity.CountyParishHolding.Identifier,
            DelegatingUserId = entity.DelegatingUserId,
            DelegatingUserName = entity.DelegatingUser.DisplayName,
            DelegatedUserId = entity.DelegatedUserId,
            DelegatedUserName = entity.DelegatedUser?.DisplayName,
            DelegatedUserEmail = entity.DelegatedUserEmail,
            DelegatedUserRoleId = entity.DelegatedUserRoleId,
            DelegatedUserRoleName = entity.DelegatedUserRole.Name,
            InvitationExpiresAt = entity.InvitationExpiresAt,
            InvitationAcceptedAt = entity.InvitationAcceptedAt,
            InvitationRejectedAt = entity.InvitationRejectedAt,
            RevokedAt = entity.RevokedAt,
            ExpiresAt = entity.ExpiresAt,
            RevokedById = entity.RevokedById,
            RevokedByName = entity.RevokedByUser?.DisplayName,
        };
    }
}
