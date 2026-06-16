// <copyright file="CphDelegationService.cs" company="Defra">
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
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Common.Extensions;
using Defra.Identity.Services.Common.Filters;
using Defra.Identity.Services.Common.Mappers;
using Defra.Identity.Services.Common.Strategy.Factories;
using Defra.Identity.Services.Delegations.Injection;
using Defra.Identity.Services.Delegations.Rules;
using FluentValidation;
using Microsoft.Extensions.Logging;

public partial class CphDelegationService : ICphDelegationService
{
    private const string CreateDelegationActionDescription = "Create county parish holding delegation";

    private readonly ICphDelegationsRepository delegationRepository;
    private readonly IUserRepository userRepository;
    private readonly ICphRepository cphRepository;
    private readonly IRoleRepository roleRepository;
    private readonly IOperatorContext operatorContext;
    private readonly IStrategyBuilderFactory<CphDelegationService> strategyBuilderFactory;
    private readonly IValidator<CreateCphDelegation> createCphDelegationValidator;
    private readonly ILogger<CphDelegationService> logger;
    private readonly IMessagingFactory messagingFactory;

    public CphDelegationService(
        ICphDelegationSvcRepoContext repoContext,
        IOperatorContext operatorContext,
        IMessagingFactory messagingFactory,
        IStrategyBuilderFactory<CphDelegationService> strategyBuilderFactory,
        IValidator<CreateCphDelegation> createCphDelegationValidator,
        ILogger<CphDelegationService> logger)
    {
        this.delegationRepository = repoContext.DelegationRepository;
        this.userRepository = repoContext.UserRepository;
        this.cphRepository = repoContext.CphRepository;
        this.roleRepository = repoContext.RoleRepository;
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
            .WithRepository(delegationRepository)
            .WithCancellationToken(cancellationToken)
            .WithEntityFilter(FilterLibrary.CphDelegations
                .NotSoftDeletedOrExpiredAndValidRefsAndHasValidCphOwnerAssignment)
            .ExecuteAndMap(DelegationMapper.MapCphDelegationEntityToCphDelegation);
    }

    public async Task<CphDelegation> Get(GetCphDelegationById request, CancellationToken cancellationToken = default)
    {
        var delegationFilter = FilterLibrary.CphDelegations
            .NotSoftDeletedOrExpiredAndValidRefsAndHasValidCphOwnerAssignment
            .AndAlso(delegation => request.Id == delegation.Id);

        return await strategyBuilderFactory.BuildGetStrategy<CountyParishHoldingDelegations>()
            .WithActionDescription("Get county parish holding delegation")
            .WithRepository(delegationRepository)
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
            .WithRepository(delegationRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestValidation(() => createCphDelegationValidator.ValidateAsync(request, cancellationToken))
            .WithReferenceRules(rules =>
            {
                rules.Add(
                    cphRepository,
                    FilterLibrary.Cphs.NotSoftDeletedOrExpired.AndAlso(cph =>
                        cph.Id == request.CountyParishHoldingId),
                    RulesLibrary.Reference.Descriptions.CphMustExistNotDeletedOrExpired);
                rules.Add(
                    userRepository,
                    FilterLibrary.Users.NotSoftDeleted.AndAlso(user => user.Id == request.DelegatingUserId),
                    RulesLibrary.Reference.Descriptions.DelegatingUserMustExistNotDeleted);
                rules.Add(
                    roleRepository,
                    role => role.Id == request.DelegatedUserRoleId,
                    RulesLibrary.Reference.Descriptions.DelegatedUserRoleMustExist);
            })
            .WithCreate(() => new CountyParishHoldingDelegations
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
            .WithAfterExecute(async delegation =>
            {
                await messagingFactory
                    .QueueDelegationEmailAsync(
                        new DelegationEmailMessage(delegation.Id, MessageTemplateTypes.Delegation.DelegationInvitee)
                        {
                            Recipient = delegation.DelegatedUserEmail,
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
            .WithRepository(delegationRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(delegation => request.Id == delegation.Id)
            .WithBusinessRules(rules =>
            {
                rules.Add(RulesLibrary.Business.DelegationNotSoftDeleted);
                rules.Add(RulesLibrary.Business.DelegationNotExpired);
                rules.Add(RulesLibrary.Business.InvitationNotAccepted);
                rules.Add(RulesLibrary.Business.InvitationNotRejected);
                rules.Add(RulesLibrary.Business.DelegationNotRevoked);
                rules.Add(RulesLibrary.Business.InvitationNotExpired);
            })
            .WithBeforeUpdate(ValidateOperatorCanAcceptOrRejectInvitation)
            .WithUpdate(delegation => { delegation.InvitationAcceptedAt = DateTime.UtcNow; })
            .WithAfterExecute(async delegation =>
                await messagingFactory
                    .QueueDelegationEmailAsync(
                        new DelegationEmailMessage(
                            delegation.Id,
                            MessageTemplateTypes.Delegation.DelegationInviterConfirmation)
                        {
                            Recipient = delegation.DelegatingUser.EmailAddress,
                        },
                        cancellationToken)
                    .ConfigureAwait(false))
            .Execute();
    }

    public async Task Reject(RejectCphDelegationById request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy<CountyParishHoldingDelegations>()
            .WithActionDescription("Reject county parish holding delegation invite")
            .WithRepository(delegationRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(delegation => request.Id == delegation.Id)
            .WithBusinessRules(rules =>
            {
                rules.Add(RulesLibrary.Business.DelegationNotSoftDeleted);
                rules.Add(RulesLibrary.Business.DelegationNotExpired);
                rules.Add(RulesLibrary.Business.InvitationNotAccepted);
                rules.Add(RulesLibrary.Business.InvitationNotRejected);
                rules.Add(RulesLibrary.Business.DelegationNotRevoked);
                rules.Add(RulesLibrary.Business.InvitationNotExpired);
            })
            .WithBeforeUpdate(ValidateOperatorCanAcceptOrRejectInvitation)
            .WithUpdate(delegation => { delegation.InvitationRejectedAt = DateTime.UtcNow; })
            .WithAfterExecute(async delegation =>
                await messagingFactory
                    .QueueDelegationEmailAsync(
                        new DelegationEmailMessage(
                            delegation.Id,
                            MessageTemplateTypes.Delegation.DelegationInviterConfirmation)
                        {
                            Recipient = delegation.DelegatingUser.EmailAddress,
                        },
                        cancellationToken)
                    .ConfigureAwait(false))
            .Execute();
    }

    public async Task Revoke(RevokeCphDelegationById request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy<CountyParishHoldingDelegations>()
            .WithActionDescription("Revoke county parish holding delegation")
            .WithRepository(delegationRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(delegation => request.Id == delegation.Id)
            .WithBusinessRules(rules =>
            {
                rules.Add(RulesLibrary.Business.DelegationNotSoftDeleted);
                rules.Add(RulesLibrary.Business.DelegationNotRevoked);
            })
            .WithUpdate(delegation =>
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
            .WithRepository(delegationRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(delegation => request.Id == delegation.Id)
            .WithBusinessRules(rules =>
            {
                rules.Add(RulesLibrary.Business.DelegationNotSoftDeleted);
                rules.Add(RulesLibrary.Business.DelegationNotExpired);
            })
            .WithUpdate(delegation => { delegation.ExpiresAt = DateTime.UtcNow; })
            .Execute();
    }

    public async Task Delete(DeleteCphDelegationById request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy<CountyParishHoldingDelegations>()
            .WithActionDescription("Delete county parish holding delegation")
            .WithRepository(delegationRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(delegation => request.Id == delegation.Id)
            .WithBusinessRules(rules => { rules.Add(RulesLibrary.Business.DelegationNotSoftDeleted); })
            .WithUpdate(cph =>
            {
                cph.DeletedAt = DateTime.UtcNow;
                cph.DeletedById = operatorContext.OperatorId;
            })
            .Execute();
    }

    private async Task<UserAccounts> GetDelegatedUserByEmailAddress(
        CreateCphDelegation request,
        CancellationToken cancellationToken)
    {
        var lowerCaseEmailAddress = request.DelegatedUserEmail.ToLowerInvariant();

        var delegatedUser = await userRepository.GetSingle(
            user => user.EmailAddress == lowerCaseEmailAddress,
            cancellationToken);

        if (delegatedUser is { DeletedAt: null })
        {
            return delegatedUser;
        }

        LogDelegatedUserNotFound(
            logger,
            CreateDelegationActionDescription.ToLowerInvariant(),
            EntityDescriptions.CphDelegation.ToLowerInvariant(),
            RulesLibrary.Reference.Descriptions.DelegatedUserMustExistNotDeleted);

        throw new NotFoundException(RulesLibrary.Reference.Descriptions.DelegatedUserMustExistNotDeleted);
    }

    private Task ValidateOperatorCanAcceptOrRejectInvitation(CountyParishHoldingDelegations delegation)
    {
        try
        {
            if (delegation.DelegatedUserId.HasValue && delegation.DelegatedUserId == operatorContext.OperatorId)
            {
                return Task.CompletedTask;
            }

            LogAcceptOrRejectInvitationFailedAuthorisationRules(logger, delegation.Id, operatorContext.OperatorId);

            throw new UnauthorizedAccessException("Operator cannot accept or reject this delegation");
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }
}
