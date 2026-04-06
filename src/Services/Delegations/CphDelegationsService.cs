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
using Defra.Identity.Requests.Delegations.Commands.Accept;
using Defra.Identity.Requests.Delegations.Commands.Create;
using Defra.Identity.Requests.Delegations.Commands.Delete;
using Defra.Identity.Requests.Delegations.Commands.Expire;
using Defra.Identity.Requests.Delegations.Commands.Reject;
using Defra.Identity.Requests.Delegations.Commands.Revoke;
using Defra.Identity.Requests.Delegations.Commands.Update;
using Defra.Identity.Requests.Delegations.Queries;
using Defra.Identity.Responses.Delegations;
using Defra.Identity.Services.Common.Builders.Strategy.Factories;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Delegations.Rules;
using FluentValidation;
using Microsoft.Extensions.Logging;

public class CphDelegationsService : ICphDelegationsService
{
    private readonly ICphDelegationsRepository repository;
    private readonly IUsersRepository usersRepository;
    private readonly ICphRepository cphRepository;
    private readonly IRoleRepository roleRepository;
    private readonly IOperatorContext operatorContext;
    private readonly IStrategyBuilderFactory<CphDelegationsService, CountyParishHoldingDelegations> strategyBuilderFactory;
    private readonly IValidator<CreateCphDelegation> createCphDelegationValidator;
    private readonly IValidator<UpdateCphDelegationById> updateCphDelegationValidator;
    private readonly ILogger<CphDelegationsService> logger;

    public CphDelegationsService(
        ICphDelegationsRepository repository,
        IUsersRepository usersRepository,
        ICphRepository cphRepository,
        IRoleRepository roleRepository,
        IOperatorContext operatorContext,
        IStrategyBuilderFactory<CphDelegationsService, CountyParishHoldingDelegations> strategyBuilderFactory,
        IValidator<CreateCphDelegation> createCphDelegationValidator,
        IValidator<UpdateCphDelegationById> updateCphDelegationValidator,
        ILogger<CphDelegationsService> logger)
    {
        this.repository = repository;
        this.usersRepository = usersRepository;
        this.cphRepository = cphRepository;
        this.roleRepository = roleRepository;
        this.operatorContext = operatorContext;
        this.strategyBuilderFactory = strategyBuilderFactory;
        this.createCphDelegationValidator = createCphDelegationValidator;
        this.updateCphDelegationValidator = updateCphDelegationValidator;
        this.logger = logger;

        this.strategyBuilderFactory
            .WithLogger(this.logger)
            .WithOperatorContext(this.operatorContext)
            .WithEntityDescription("County parish holding delegation");
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

    public async Task<CphDelegation> Create(CreateCphDelegation request, CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildCreateStrategy(repository, "Create")
            .WithCancellationToken(cancellationToken)
            .WithRequestValidation(() => createCphDelegationValidator.ValidateAsync(request, cancellationToken))
            .WithReferenceRules(
                rules =>
                {
                    rules.Add(cphRepository, request.CountyParishHoldingId, RulesLibrary.Reference.Descriptions.CphMustExistNotDeletedOrExpired);
                    rules.Add(usersRepository, request.DelegatingUserId, RulesLibrary.Reference.Descriptions.DelegatingUserMustExistNotDeleted);
                    rules.Add(usersRepository, request.DelegatedUserId, RulesLibrary.Reference.Descriptions.DelegatedUserMustExistNotDeleted);
                    rules.Add(roleRepository, request.DelegatedUserRoleId, RulesLibrary.Reference.Descriptions.DelegatedUserRoleMustExistNotDeleted);
                })
            .WithCreate(
                () =>
                    new CountyParishHoldingDelegations
                    {
                        CountyParishHoldingId = request.CountyParishHoldingId,
                        DelegatingUserId = request.DelegatingUserId,
                        DelegatedUserId = request.DelegatedUserId,
                        DelegatedUserEmail = request.DelegatedUserEmail,
                        DelegatedUserRoleId = request.DelegatedUserRoleId,
                        InvitationToken = string.Empty,
                        InvitationExpiresAt = DateTime.Now.AddDays(2).ToUniversalTime(),
                        CreatedAt = DateTime.Now.ToUniversalTime(),
                        CreatedById = operatorContext.OperatorId,
                    })
            .ExecuteAndMap(MapToResponse);
    }

    public async Task<CphDelegation> Update(UpdateCphDelegationById request, CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildUpdateStrategy(repository, "Update")
            .WithCancellationToken(cancellationToken)
            .WithRequestValidation(() => updateCphDelegationValidator.ValidateAsync(request, cancellationToken))
            .WithRequestAndEntityFilter(request, delegation => request.Id == delegation.Id)
            .WithExistenceRules(
                rules =>
                {
                    rules.Add(RulesLibrary.Existence.NotSoftDeleted);
                    rules.Add(RulesLibrary.Existence.NotExpired);
                })
            .WithReferenceRules(
                rules =>
                {
                    rules.Add(cphRepository, request.CountyParishHoldingId, RulesLibrary.Reference.Descriptions.CphMustExistNotDeletedOrExpired);
                    rules.Add(usersRepository, request.DelegatingUserId, RulesLibrary.Reference.Descriptions.DelegatingUserMustExistNotDeleted);
                    rules.Add(usersRepository, request.DelegatedUserId, RulesLibrary.Reference.Descriptions.DelegatedUserMustExistNotDeleted);
                    rules.Add(roleRepository, request.DelegatedUserRoleId, RulesLibrary.Reference.Descriptions.DelegatedUserRoleMustExistNotDeleted);
                })
            .WithBusinessRules(
                rules =>
                {
                    rules.Add(RulesLibrary.Business.InvitationNotExpired);
                    rules.Add(RulesLibrary.Business.InvitationNotAccepted);
                    rules.Add(RulesLibrary.Business.InvitationNotRejected);
                    rules.Add(RulesLibrary.Business.NotRevoked);
                })
            .WithUpdate(
                delegation =>
                {
                    delegation.CountyParishHoldingId = request.CountyParishHoldingId;
                    delegation.DelegatingUserId = request.DelegatingUserId;
                    delegation.DelegatedUserId = request.DelegatedUserId;
                    delegation.DelegatedUserEmail = request.DelegatedUserEmail;
                    delegation.DelegatedUserRoleId = request.DelegatedUserRoleId;
                })
            .ExecuteAndMap(MapToResponse);
    }

    public async Task Accept(AcceptCphDelegationById request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy(repository, "Accept")
            .WithCancellationToken(cancellationToken)
            .WithRequestAndEntityFilter(request, delegation => request.Id == delegation.Id)
            .WithExistenceRules(
                rules =>
                {
                    rules.Add(RulesLibrary.Existence.NotSoftDeleted);
                    rules.Add(RulesLibrary.Existence.NotExpired);
                })
            .WithBusinessRules(
                rules =>
                {
                    rules.Add(RulesLibrary.Business.InvitationNotExpired);
                    rules.Add(RulesLibrary.Business.InvitationNotAccepted);
                    rules.Add(RulesLibrary.Business.InvitationNotRejected);
                    rules.Add(RulesLibrary.Business.NotRevoked);
                })
            .WithUpdate(delegation => { delegation.InvitationAcceptedAt = DateTime.Now.ToUniversalTime(); })
            .Execute();
    }

    public async Task Reject(RejectCphDelegationById request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy(repository, "Reject")
            .WithCancellationToken(cancellationToken)
            .WithRequestAndEntityFilter(request, delegation => request.Id == delegation.Id)
            .WithExistenceRules(
                rules =>
                {
                    rules.Add(RulesLibrary.Existence.NotSoftDeleted);
                    rules.Add(RulesLibrary.Existence.NotExpired);
                })
            .WithBusinessRules(
                rules =>
                {
                    rules.Add(RulesLibrary.Business.InvitationNotExpired);
                    rules.Add(RulesLibrary.Business.InvitationNotAccepted);
                    rules.Add(RulesLibrary.Business.InvitationNotRejected);
                    rules.Add(RulesLibrary.Business.NotRevoked);
                })
            .WithUpdate(delegation => { delegation.InvitationRejectedAt = DateTime.Now.ToUniversalTime(); })
            .Execute();
    }

    public async Task Revoke(RevokeCphDelegationById request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy(repository, "Revoke")
            .WithCancellationToken(cancellationToken)
            .WithRequestAndEntityFilter(request, delegation => request.Id == delegation.Id)
            .WithExistenceRules(rules => { rules.Add(RulesLibrary.Existence.NotSoftDeleted); })
            .WithBusinessRules(rules => { rules.Add(RulesLibrary.Business.NotRevoked); })
            .WithUpdate(
                delegation =>
                {
                    delegation.RevokedAt = DateTime.Now.ToUniversalTime();
                    delegation.RevokedById = operatorContext.OperatorId;
                })
            .Execute();
    }

    public async Task Expire(ExpireCphDelegationById request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy(repository, "Expire")
            .WithCancellationToken(cancellationToken)
            .WithRequestAndEntityFilter(request, delegation => request.Id == delegation.Id)
            .WithExistenceRules(
                rules =>
                {
                    rules.Add(RulesLibrary.Existence.NotSoftDeleted);
                    rules.Add(RulesLibrary.Existence.NotExpired);
                })
            .WithUpdate(delegation => { delegation.ExpiresAt = DateTime.Now.ToUniversalTime(); })
            .Execute();
    }

    public async Task<bool> Delete(DeleteCphDelegationById request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting delegation with id {Id} by operator {OperatorId}", request.Id, operatorContext.OperatorId);
        return await repository.Delete(x => x.Id == request.Id, operatorContext.OperatorId, cancellationToken);
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
