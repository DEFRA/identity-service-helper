// <copyright file="DelegationsService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Applications;
using Defra.Identity.Repositories.Delegates;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Requests.Delegations.Commands.Create;
using Defra.Identity.Requests.Delegations.Commands.Update;
using Defra.Identity.Requests.Delegations.Queries;
using Defra.Identity.Responses.Delegations;
using Microsoft.Extensions.Logging;

public class DelegationsService : IDelegationsService
{
    private readonly IDelegatesRepository repository;
    private readonly IUsersRepository usersRepository;
    private readonly IApplicationsRepository applicationsRepository;
    private readonly ILogger<DelegationsService> logger;

    public DelegationsService(
        IDelegatesRepository repository,
        IUsersRepository usersRepository,
        IApplicationsRepository applicationsRepository,
        ILogger<DelegationsService> logger)
    {
        this.repository = repository;
        this.usersRepository = usersRepository;
        this.applicationsRepository = applicationsRepository;
        this.logger = logger;
    }

    public async Task<List<Delegation>> GetAll(GetDelegations request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all delegations");
        var entities = await repository.GetList(x => true, cancellationToken);

        return entities.Select(MapToResponse).ToList();
    }

    public async Task<Delegation> Get(GetDelegationById request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting delegation by id {Id}", request.Id);
        Expression<Func<Delegations, bool>> filter = x => x.Id == request.Id;

        var entity = await repository.GetSingle(filter, cancellationToken);
        if (entity == null)
        {
            logger.LogWarning("Delegation with id {Id} not found", request.Id);
            throw new NotFoundException("Delegation not found.");
        }

        return MapToResponse(entity);
    }

    public async Task<Delegation> Update(UpdateDelegation request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating delegation with id {Id}", request.Id);
        var existing = await repository.GetSingle(x => x.Id.Equals(request.Id), cancellationToken);
        if (existing == null)
        {
            logger.LogWarning("Delegation with id {Id} not found for update", request.Id);
            throw new NotFoundException($"Delegation with id {request.Id} not found.");
        }

        await ValidateReferences(request.ApplicationId, request.UserId, cancellationToken);

        existing.ApplicationId = request.ApplicationId;
        existing.UserId = request.UserId;

        var updated = await repository.Update(existing, cancellationToken);
        return MapToResponse(updated);
    }

    public async Task<Delegation> Create(CreateDelegation request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new delegation for application {ApplicationId} and user {UserId}", request.ApplicationId, request.UserId);

        await ValidateReferences(request.ApplicationId, request.UserId, cancellationToken);

        var entity = new Delegations
        {
            ApplicationId = request.ApplicationId,
            UserId = request.UserId,
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

    private async Task ValidateReferences(Guid applicationId, Guid userId, CancellationToken cancellationToken)
    {
        var application = await applicationsRepository.GetSingle(x => x.Id == applicationId, cancellationToken);
        if (application == null)
        {
            logger.LogWarning("Application with id {Id} not found", applicationId);
            throw new NotFoundException($"Application with id {applicationId} not found.");
        }

        var user = await usersRepository.GetSingle(x => x.Id == userId, cancellationToken);
        if (user == null)
        {
            logger.LogWarning("User with id {Id} not found", userId);
            throw new NotFoundException($"User with id {userId} not found.");
        }
    }

    private static Delegation MapToResponse(Delegations entity)
    {
        return new Delegation
        {
            Id = entity.Id,
            ApplicationId = entity.ApplicationId,
            UserId = entity.UserId,
        };
    }
}
