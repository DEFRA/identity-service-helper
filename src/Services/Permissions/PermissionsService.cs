// <copyright file="PermissionsService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Permissions;

using Defra.Identity.Models.Requests.Permissions.Queries;
using Defra.Identity.Models.Responses.Assignments;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Models.Responses.Delegations;
using Defra.Identity.Models.Responses.Permissions;
using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Assignments;
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
    private readonly ICphAssignmentsForAssigneeRepository cphAssignmentsForAssigneeRepository;
    private readonly ICphDelegationsForDelegateRepository cphDelegationsForDelegateRepository;
    private readonly ICphDelegatesForCphAssigneeRepository cphDelegatesForCphAssigneeRepository;
    private readonly IStrategyBuilderFactory<PermissionsService> strategyBuilderFactory;

    public PermissionsService(
        IUsersRepository userRepository,
        ICphAssignmentsForAssigneeRepository cphAssignmentsForAssigneeRepository,
        ICphDelegationsForDelegateRepository cphDelegationsForDelegateRepository,
        ICphDelegatesForCphAssigneeRepository cphDelegatesForCphAssigneeRepository,
        IStrategyBuilderFactory<PermissionsService> strategyBuilderFactory,
        ILogger<PermissionsService> logger)
    {
        this.userRepository = userRepository;
        this.cphAssignmentsForAssigneeRepository = cphAssignmentsForAssigneeRepository;
        this.cphDelegationsForDelegateRepository = cphDelegationsForDelegateRepository;
        this.cphDelegatesForCphAssigneeRepository = cphDelegatesForCphAssigneeRepository;
        this.strategyBuilderFactory = strategyBuilderFactory;

        this.cphDelegatesForCphAssigneeRepository
            .WithHoldingAssignmentsFilter(FiltersLibrary.CphAssignments.NotSoftDeleted)
            .WithCountyParishHoldingsFilter(FiltersLibrary.Cphs.NotSoftDeletedOrExpired)
            .WithDelegationsFilter(FiltersLibrary.CphDelegations.ActiveDelegation);

        this.strategyBuilderFactory
            .WithDefaultLogger(logger);
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
            .WithAssociatedEntityFilter(FiltersLibrary.CphAssignments.ActiveAssignment)
            .WithPrimaryEntityExistenceRules(rules => { rules.Add(RulesLibrary.Existence.NotSoftDeleted); })
            .ExecuteAndMap(
                entity => new CphAssignment()
                {
                    Id = entity.Id,
                    CountyParishHoldingId = entity.CountyParishHoldingId,
                    CountyParishHoldingNumber = entity.CountyParishHolding.Identifier,
                    UserId = entity.UserAccountId,
                    ApplicationId = entity.ApplicationId,
                    RoleId = entity.RoleId,
                    RoleName = entity.Role.Name,
                    Email = entity.UserAccount.EmailAddress,
                    DisplayName = entity.UserAccount.DisplayName,
                });

        var userDelegatedCphs = await strategyBuilderFactory.BuildGetAssociationsListStrategy<UserAccounts, CountyParishHoldingDelegations>()
            .WithPrimaryEntityDescription("User account")
            .WithActionDescription("Get county parish holding delegations which are delegated to")
            .WithPrimaryRepository(userRepository)
            .WithAssociationsRepository(cphDelegationsForDelegateRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestAndPrimaryEntityFilter(request, userAccount => userAccount.Id == request.Id)
            .WithAssociatedEntityFilter(FiltersLibrary.CphDelegations.ActiveDelegation)
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
            .WithAssociatedEntityFilter(FiltersLibrary.Users.NotSoftDeleted)
            .WithPrimaryEntityExistenceRules(rules => { rules.Add(RulesLibrary.Existence.NotSoftDeleted); })
            .ExecuteAndMap(MapUserEntityToUser, SelectorLibrary.UserDisplayName);
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
