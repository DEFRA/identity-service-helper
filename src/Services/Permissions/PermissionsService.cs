// <copyright file="PermissionsService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Permissions;

using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Cphs.Queries;
using Defra.Identity.Models.Requests.Permissions.Queries;
using Defra.Identity.Models.Responses.Assignments;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Models.Responses.Delegations;
using Defra.Identity.Models.Responses.Permissions;
using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Assignments;
using Defra.Identity.Repositories.Common;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Services.Common.Builders.Strategy.Factories;
using Defra.Identity.Services.Permissions.Filters;
using Defra.Identity.Services.Permissions.Helpers;
using Defra.Identity.Services.Permissions.Rules;
using Defra.Identity.Services.Permissions.Selectors;
using Microsoft.Extensions.Logging;

public class PermissionsService : IPermissionsService
{
    private readonly IUsersRepository userRepository;
    private readonly ICphRepository cphRepository;
    private readonly ICphAssignmentsRepository cphAssignmentsRepository;
    private readonly ICphAssignmentsForAssigneeRepository cphAssignmentsForAssigneeRepository;
    private readonly ICphDelegationsForDelegateRepository cphDelegationsForDelegateRepository;
    private readonly ICphDelegatesForCphAssigneeRepository cphDelegatesForCphAssigneeRepository;
    private readonly IStrategyBuilderFactory<PermissionsService> strategyBuilderFactory;
    private readonly ILogger<PermissionsService> logger;

    public PermissionsService(
        IUsersRepository userRepository,
        ICphRepository cphRepository,
        ICphAssignmentsRepository cphAssignmentsRepository,
        ICphAssignmentsForAssigneeRepository cphAssignmentsForAssigneeRepository,
        ICphDelegationsForDelegateRepository cphDelegationsForDelegateRepository,
        ICphDelegatesForCphAssigneeRepository cphDelegatesForCphAssigneeRepository,
        IStrategyBuilderFactory<PermissionsService> strategyBuilderFactory,
        ILogger<PermissionsService> logger)
    {
        this.userRepository = userRepository;
        this.cphRepository = cphRepository;
        this.cphAssignmentsRepository = cphAssignmentsRepository;
        this.cphAssignmentsForAssigneeRepository = cphAssignmentsForAssigneeRepository;
        this.cphDelegationsForDelegateRepository = cphDelegationsForDelegateRepository;
        this.cphDelegatesForCphAssigneeRepository = cphDelegatesForCphAssigneeRepository;
        this.strategyBuilderFactory = strategyBuilderFactory;
        this.logger = logger;

        this.cphDelegatesForCphAssigneeRepository
            .WithHoldingAssignmentsFilter(FilterLibrary.CphAssignments.NotSoftDeleted)
            .WithCountyParishHoldingsFilter(FilterLibrary.Cphs.NotSoftDeletedOrExpired)
            .WithDelegationsFilter(FilterLibrary.CphDelegations.ActiveDelegation);

        this.strategyBuilderFactory
            .WithDefaultLogger(logger);
    }

    public async Task<PagedResults<CphAssignment>> GetCphAssignments(GetCphAssignmentsByCphId request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all county parish holding users for id {Id} by page", request.Id);

        Expression<Func<CountyParishHoldings, bool>> primaryFilter = cph => cph.Id == request.Id;
        Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> associationFilter = cphUser => cphUser.DeletedAt == null;
        Expression<Func<ApplicationUserAccountHoldingAssignments, string>> orderBy = cphUser => cphUser.UserAccount.DisplayName;

        var cphEntity = await cphRepository.GetSingle(primaryFilter, cancellationToken);

        if (cphEntity is not { DeletedAt: null } || cphEntity.ExpiredAt != null)
        {
            logger.LogWarning("County parish holding with id {Id} not found", request.Id);

            throw new NotFoundException("County parish holding not found.");
        }

        var pagedCphAssignmentEntities = await cphAssignmentsRepository.GetPaged(
            primaryFilter,
            associationFilter,
            request.PageNumber,
            request.PageSize,
            orderBy,
            request.OrderByDescending ?? false,
            cancellationToken);

        var pagedCphUserResults = pagedCphAssignmentEntities.ToPagedResults(MapCphAssignmentEntityToCphAssignment);

        return pagedCphUserResults;
    }

    public async Task<UserCphs> GetUserCphs(GetUserCphsByUserId request, CancellationToken cancellationToken = default)
    {
        var userAssociatedCphs = await strategyBuilderFactory.BuildGetAssociationsListStrategy<UserAccounts, ApplicationUserAccountHoldingAssignments>()
            .WithPrimaryEntityDescription("User account")
            .WithActionDescription("Get all county parish holdings assigned to")
            .WithPrimaryRepository(userRepository)
            .WithAssociationsRepository(cphAssignmentsForAssigneeRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestAndPrimaryEntityFilter(request, userAccount => userAccount.Id == request.Id)
            .WithAssociatedEntityFilter(FilterLibrary.CphAssignments.ActiveAssignment)
            .WithPrimaryEntityExistenceRules(rules => { rules.Add(RulesLibrary.Existence.NotSoftDeleted); })
            .ExecuteAndMap(MapCphAssignmentEntityToCphAssignment);

        var userDelegatedCphs = await strategyBuilderFactory.BuildGetAssociationsListStrategy<UserAccounts, CountyParishHoldingDelegations>()
            .WithPrimaryEntityDescription("User account")
            .WithActionDescription("Get county parish holding delegations which are delegated to")
            .WithPrimaryRepository(userRepository)
            .WithAssociationsRepository(cphDelegationsForDelegateRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestAndPrimaryEntityFilter(request, userAccount => userAccount.Id == request.Id)
            .WithAssociatedEntityFilter(FilterLibrary.CphDelegations.ActiveDelegation)
            .WithPrimaryEntityExistenceRules(rules => { rules.Add(RulesLibrary.Existence.NotSoftDeleted); })
            .ExecuteAndMap(MapCphDelegationEntityToCphDelegation);

        return new UserCphs(userAssociatedCphs, userDelegatedCphs);
    }

    public async Task<PagedResults<User>> GetUserDelegates(GetUserDelegatesById request, CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildGetAssociationsPagedStrategy<UserAccounts, UserAccounts>()
            .WithPrimaryEntityDescription("User account")
            .WithActionDescription("Get county parish holding unique delegates associated with assigned cphs for")
            .WithPrimaryRepository(userRepository)
            .WithAssociationsRepository(cphDelegatesForCphAssigneeRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestAndPrimaryEntityFilter(request, userAccount => userAccount.Id == request.Id)
            .WithAssociatedEntityFilter(FilterLibrary.Users.NotSoftDeleted)
            .WithPrimaryEntityExistenceRules(rules => { rules.Add(RulesLibrary.Existence.NotSoftDeleted); })
            .ExecuteAndMap(MapUserEntityToUser, SelectorLibrary.UserDisplayName);
    }

    private static CphAssignment MapCphAssignmentEntityToCphAssignment(ApplicationUserAccountHoldingAssignments cphAssignmentEntity)
    {
        return new CphAssignment
        {
            Id = cphAssignmentEntity.Id,
            CountyParishHoldingId = cphAssignmentEntity.CountyParishHoldingId,
            CountyParishHoldingNumber = cphAssignmentEntity.CountyParishHolding.Identifier,
            UserId = cphAssignmentEntity.UserAccountId,
            ApplicationId = cphAssignmentEntity.ApplicationId,
            RoleId = cphAssignmentEntity.RoleId,
            RoleName = cphAssignmentEntity.Role.Name,
            Email = cphAssignmentEntity.UserAccount.EmailAddress,
            DisplayName = cphAssignmentEntity.UserAccount.DisplayName,
        };
    }

    private static CphDelegation MapCphDelegationEntityToCphDelegation(CountyParishHoldingDelegations cphDelegationEntity)
    {
        return new CphDelegation()
        {
            Id = cphDelegationEntity.Id,
            CountyParishHoldingId = cphDelegationEntity.CountyParishHoldingId,
            CountyParishHoldingNumber = cphDelegationEntity.CountyParishHolding.Identifier,
            DelegatingUserId = cphDelegationEntity.DelegatingUserId,
            DelegatingUserName = cphDelegationEntity.DelegatingUser.DisplayName,
            DelegatedUserId = cphDelegationEntity.DelegatedUserId,
            DelegatedUserName = cphDelegationEntity.DelegatedUser?.DisplayName,
            DelegatedUserEmail = cphDelegationEntity.DelegatedUserEmail,
            DelegatedUserRoleId = cphDelegationEntity.DelegatedUserRoleId,
            DelegatedUserRoleName = cphDelegationEntity.DelegatedUserRole.Name,
            InvitationExpiresAt = cphDelegationEntity.InvitationExpiresAt,
            InvitationAcceptedAt = cphDelegationEntity.InvitationAcceptedAt,
            InvitationRejectedAt = cphDelegationEntity.InvitationRejectedAt,
            RevokedAt = cphDelegationEntity.RevokedAt,
            ExpiresAt = cphDelegationEntity.ExpiresAt,
            RevokedById = cphDelegationEntity.RevokedById,
            RevokedByName = cphDelegationEntity.RevokedByUser?.DisplayName,
            Active = PermissionsHelper.IsActiveDelegation(cphDelegationEntity),
        };
    }

    private static User MapUserEntityToUser(UserAccounts userEntity)
    {
        return new User()
        {
            Id = userEntity.Id,
            Email = userEntity.EmailAddress,
            FirstName = userEntity.FirstName,
            LastName = userEntity.LastName,
            DisplayName = userEntity.DisplayName,
        };
    }
}
