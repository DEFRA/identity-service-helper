// <copyright file="CphDelegationsService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations;

using Defra.Identity.Messaging;
using Defra.Identity.Messaging.Models.Request;
using Defra.Identity.Messaging.Services;
using Defra.Identity.Models.Requests.Delegations.Commands;
using Defra.Identity.Models.Requests.Delegations.Queries;
using Defra.Identity.Models.Responses.Delegations;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Roles;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Services.Common;
using Defra.Identity.Services.Common.Builders.Predicates.Models;
using Defra.Identity.Services.Common.Builders.Strategy.Factories;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Common.Exceptions;
using Defra.Identity.Services.Common.Extensions;
using Defra.Identity.Services.Common.Filters;
using Defra.Identity.Services.Common.Mappers;
using Defra.Identity.Services.Delegations.Rules;
using FluentValidation;
using Microsoft.Extensions.Logging;

public partial class CphDelegationsService : ICphDelegationsService
{
    private const string CreateDelegationActionDescription = "Create county parish holding delegation";

    private readonly ICphDelegationsRepository repository;
    private readonly IUsersRepository usersRepository;
    private readonly ICphRepository cphRepository;
    private readonly IRoleRepository roleRepository;
    private readonly IOperatorContext operatorContext;
    private readonly IStrategyBuilderFactory<CphDelegationsService> strategyBuilderFactory;
    private readonly IValidator<CreateCphDelegation> createCphDelegationValidator;
    private readonly ILogger<CphDelegationsService> logger;
    private readonly IMessagingFactory messagingFactory;

    public CphDelegationsService(
        ICphDelegationsRepository repository,
        IUsersRepository usersRepository,
        ICphRepository cphRepository,
        IRoleRepository roleRepository,
        IOperatorContext operatorContext,
        IMessagingFactory messagingFactory,
        IStrategyBuilderFactory<CphDelegationsService> strategyBuilderFactory,
        IValidator<CreateCphDelegation> createCphDelegationValidator,
        ILogger<CphDelegationsService> logger)
    {
        this.repository = repository;
        this.usersRepository = usersRepository;
        this.cphRepository = cphRepository;
        this.roleRepository = roleRepository;
        this.operatorContext = operatorContext;
        this.messagingFactory = messagingFactory;
        this.strategyBuilderFactory = strategyBuilderFactory;
        this.createCphDelegationValidator = createCphDelegationValidator;
        this.logger = logger;

        this.strategyBuilderFactory
            .WithDefaultLogger(this.logger)
            .WithDefaultOperatorContext(this.operatorContext)
            .WithDefaultEntityDescription(EntityDescriptions.CphDelegation);
    }

    public async Task<List<CphDelegation>> GetAll(CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildGetListStrategy<CountyParishHoldingDelegations>()
            .WithActionDescription("Get all county parish holding delegations")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithEntityFilter(FilterLibrary.CphDelegations.NotSoftDeletedOrExpiredAndValidRefs)
            .ExecuteAndMap(DelegationMapper.MapCphDelegationEntityToCphDelegation);
    }

    public async Task<CphDelegation> Get(GetCphDelegationById request, CancellationToken cancellationToken = default)
    {
        var delegationFilter = FilterLibrary.CphDelegations.NotSoftDeletedOrExpiredAndValidRefs
            .AndAlso(delegation => request.Id == delegation.Id);

        return await strategyBuilderFactory.BuildGetStrategy<CountyParishHoldingDelegations>()
            .WithActionDescription("Get county parish holding delegation")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(delegationFilter)
            .ExecuteAndMap(DelegationMapper.MapCphDelegationEntityToCphDelegation);
    }

    public async Task<CphDelegation> Create(CreateCphDelegation request, CancellationToken cancellationToken = default)
    {
        var delegatedUser = await GetDelegatedUserByEmailAddress(request, cancellationToken);

        return await strategyBuilderFactory.BuildCreateStrategy<CountyParishHoldingDelegations>()
            .WithActionDescription("Create county parish holding delegation")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequestValidation(() => createCphDelegationValidator.ValidateAsync(request, cancellationToken))
            .WithReferenceRules(
                rules =>
                {
                    rules.Add(cphRepository, request.CountyParishHoldingId, RulesLibrary.Reference.Descriptions.CphMustExistNotDeletedOrExpired);
                    rules.Add(usersRepository, request.DelegatingUserId, RulesLibrary.Reference.Descriptions.DelegatingUserMustExistNotDeleted);
                    rules.Add(roleRepository, request.DelegatedUserRoleId, RulesLibrary.Reference.Descriptions.DelegatedUserRoleMustExistNotDeleted);
                })
            .WithCreate(
                () => new CountyParishHoldingDelegations
                {
                    CountyParishHoldingId = request.CountyParishHoldingId,
                    DelegatingUserId = request.DelegatingUserId,
                    DelegatedUserId = delegatedUser.Id,
                    DelegatedUserEmail = delegatedUser.EmailAddress,
                    DelegatedUserRoleId = request.DelegatedUserRoleId,
                    InvitationToken = string.Empty,
                    InvitationExpiresAt = DateTime.Now.AddDays(2).ToUniversalTime(),
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = operatorContext.OperatorId,
                })
            .WithAfterExecute(
                async entity =>
                {
                    await messagingFactory
                        .QueueDelegationEmailAsync(
                            new DelegationEmailMessage(entity.Id, MessageTemplateTypes.Delegation.DelegationInvitee)
                            {
                                Recipient = entity.DelegatedUserEmail,
                            },
                            cancellationToken)
                        .ConfigureAwait(false);
                })
            .ExecuteAndMap(DelegationMapper.MapCphDelegationEntityToCphDelegation);
    }

    public async Task Accept(AcceptCphDelegationById request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy<CountyParishHoldingDelegations>()
            .WithActionDescription("Accept county parish holding delegation invite")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(delegation => request.Id == delegation.Id)
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
            .WithUpdate(delegation => { delegation.InvitationAcceptedAt = DateTime.UtcNow; })
            .Execute();
    }

    public async Task AcceptInvitation(AcceptInvitationById request, CancellationToken cancellationToken = default)
    {
        await ProcessInvitationResponse(
            request,
            delegation => { delegation.InvitationAcceptedAt = DateTime.UtcNow; },
            cancellationToken);
    }

    public async Task Reject(RejectCphDelegationById request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy<CountyParishHoldingDelegations>()
            .WithActionDescription("Reject county parish holding delegation invite")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(delegation => request.Id == delegation.Id)
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
            .WithUpdate(delegation => { delegation.InvitationRejectedAt = DateTime.UtcNow; })
            .Execute();
    }

    public async Task RejectInvitation(RejectInvitationById request, CancellationToken cancellationToken = default)
    {
        await ProcessInvitationResponse(
            request,
            delegation => { delegation.InvitationRejectedAt = DateTime.UtcNow; },
            cancellationToken);
    }

    public async Task Revoke(RevokeCphDelegationById request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy<CountyParishHoldingDelegations>()
            .WithActionDescription("Revoke county parish holding delegation")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(delegation => request.Id == delegation.Id)
            .WithExistenceRules(rules => { rules.Add(RulesLibrary.Existence.NotSoftDeleted); })
            .WithBusinessRules(rules => { rules.Add(RulesLibrary.Business.NotRevoked); })
            .WithUpdate(
                delegation =>
                {
                    delegation.RevokedAt = DateTime.UtcNow;
                    delegation.RevokedById = operatorContext.OperatorId;
                })
            .Execute();
    }

    public async Task Expire(ExpireCphDelegationById request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy<CountyParishHoldingDelegations>()
            .WithActionDescription("Expire county parish holding delegation")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(delegation => request.Id == delegation.Id)
            .WithExistenceRules(
                rules =>
                {
                    rules.Add(RulesLibrary.Existence.NotSoftDeleted);
                    rules.Add(RulesLibrary.Existence.NotExpired);
                })
            .WithUpdate(delegation => { delegation.ExpiresAt = DateTime.UtcNow; })
            .Execute();
    }

    public async Task Delete(DeleteCphDelegationById request, CancellationToken cancellationToken = default)
    {
        var delegationFilter = FilterLibrary.CphDelegations.NotSoftDeleted
            .AndAlso(delegation => request.Id == delegation.Id);

        await strategyBuilderFactory.BuildUpdateStrategy<CountyParishHoldingDelegations>()
            .WithActionDescription("Delete county parish holding delegation")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(delegationFilter)
            .WithUpdate(
                cph =>
                {
                    cph.DeletedAt = DateTime.UtcNow;
                    cph.DeletedById = operatorContext.OperatorId;
                })
            .Execute();
    }

    private async Task<UserAccounts> GetDelegatedUserByEmailAddress(CreateCphDelegation request, CancellationToken cancellationToken)
    {
        var delegatedUser = await usersRepository.GetSingle(user => user.EmailAddress == request.DelegatedUserEmail, cancellationToken);

        if (delegatedUser is { DeletedAt: null })
        {
            return delegatedUser;
        }

        LogDelegatedUserNotFound(logger, CreateDelegationActionDescription, EntityDescriptions.CphDelegation, RulesLibrary.Reference.Descriptions.DelegatedUserMustExistNotDeleted);

        throw new NotFoundException(RulesLibrary.Reference.Descriptions.DelegatedUserMustExistNotDeleted);
    }

    private async Task ProcessInvitationResponse(
        InvitationResponseById request,
        Action<CountyParishHoldingDelegations> responseAction,
        CancellationToken cancellationToken)
    {
        var delegation = await repository.GetSingle(delegation => request.Id == delegation.Id, cancellationToken);

        if (delegation == null)
        {
            LogInvitationDelegationNotFound(logger, request.GetLoggableId());

            throw new NotFoundException($"{EntityDescriptions.CphDelegation} not found.");
        }

        ValidateInvitationToken(request, delegation);
        ValidateExistenceRule(request, delegation, RulesLibrary.Existence.NotSoftDeleted);
        ValidateExistenceRule(request, delegation, RulesLibrary.Existence.NotExpired);
        ValidateBusinessRule(request, delegation, RulesLibrary.Business.InvitationNotExpired);
        ValidateBusinessRule(request, delegation, RulesLibrary.Business.InvitationNotAccepted);
        ValidateBusinessRule(request, delegation, RulesLibrary.Business.InvitationNotRejected);
        ValidateBusinessRule(request, delegation, RulesLibrary.Business.NotRevoked);

        responseAction(delegation);
        delegation.InvitationToken = string.Empty;

        var updatedDelegation = await repository.Update(delegation, cancellationToken);

        await messagingFactory
            .QueueDelegationEmailAsync(
                new DelegationEmailMessage(updatedDelegation.Id, MessageTemplateTypes.Delegation.DelegationInviterConfirmation)
                {
                    Recipient = updatedDelegation.DelegatingUser.EmailAddress,
                },
                cancellationToken)
            .ConfigureAwait(false);
    }

    private void ValidateInvitationToken(InvitationResponseById request, CountyParishHoldingDelegations delegation)
    {
        if (!string.IsNullOrWhiteSpace(request.InvitationToken)
            && !string.IsNullOrWhiteSpace(delegation.InvitationToken)
            && string.Equals(delegation.InvitationToken, request.InvitationToken, StringComparison.Ordinal))
        {
            return;
        }

        LogInvitationTokenValidationFailed(logger, request.GetLoggableId());

        throw new BusinessRuleException("Invitation token is invalid");
    }

    private void ValidateExistenceRule(
        InvitationResponseById request,
        CountyParishHoldingDelegations delegation,
        EntityPredicate<CountyParishHoldingDelegations> rule)
    {
        if (rule.Predicate(delegation))
        {
            return;
        }

        LogInvitationDelegationNotFound(logger, request.GetLoggableId());

        throw new NotFoundException($"{EntityDescriptions.CphDelegation} not found.");
    }

    private void ValidateBusinessRule(
        InvitationResponseById request,
        CountyParishHoldingDelegations delegation,
        EntityPredicate<CountyParishHoldingDelegations> rule)
    {
        if (rule.Predicate(delegation))
        {
            return;
        }

        LogInvitationResponseFailedBusinessRule(logger, request.GetLoggableId(), rule.Description);

        throw new BusinessRuleException(rule.Description);
    }
}
