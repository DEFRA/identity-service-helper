// <copyright file="ProfileService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Profiles;

using Defra.Identity.Models.Requests.Profiles.Queries;
using Defra.Identity.Models.Responses.Assignments;
using Defra.Identity.Models.Responses.Delegations;
using Defra.Identity.Models.Responses.Profiles;
using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Assignments;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Services.Common;
using Defra.Identity.Services.Common.Builders.Strategy.Factories;
using Defra.Identity.Services.Common.Extensions;
using Defra.Identity.Services.Common.Filters;
using Defra.Identity.Services.Delegations.Helpers;
using Microsoft.Extensions.Logging;

public class ProfileService : IProfileService
{
    private readonly IUsersRepository userRepository;
    private readonly ICphAssignmentsRepository cphAssignmentsRepository;
    private readonly ICphDelegationsRepository cphDelegationsRepository;
    private readonly IStrategyBuilderFactory<ProfileService> strategyBuilderFactory;

    public ProfileService(
        IUsersRepository userRepository,
        ICphAssignmentsRepository cphAssignmentsRepository,
        ICphDelegationsRepository cphDelegationsRepository,
        IStrategyBuilderFactory<ProfileService> strategyBuilderFactory,
        ILogger<ProfileService> logger)
    {
        this.userRepository = userRepository;
        this.cphAssignmentsRepository = cphAssignmentsRepository;
        this.cphDelegationsRepository = cphDelegationsRepository;
        this.strategyBuilderFactory = strategyBuilderFactory;

        this.strategyBuilderFactory
            .WithDefaultLogger(logger);
    }

    public async Task<UserProfile> GetUserProfile(GetUserProfileByUserId request, CancellationToken cancellationToken = default)
    {
        var userAccountFilter =
            FilterLibrary.Users.ActiveUser
                .AndAlso(userAccount => userAccount.Id == request.Id);

        var cphAssignmentsFilter =
            FilterLibrary.CphAssignments.ActiveAssignment
                .AndAlso(assignment => assignment.UserAccountId == request.Id);

        var inboundDelegationsFilter = FilterLibrary.CphDelegations.ActiveDelegation
            .AndAlso(delegation => delegation.DelegatedUserId == request.Id);

        var outboundDelegationsFilter =
            FilterLibrary.CphDelegations.ActiveDelegation
                .AndAlso(
                    delegation
                        => delegation.CountyParishHolding.ApplicationUserAccountHoldingAssignments
                            .AsQueryable()
                            .Any(cphAssignmentsFilter));

        var userDetails = await strategyBuilderFactory.BuildGetStrategy<UserAccounts>()
            .WithEntityDescription(EntityDescriptions.User)
            .WithActionDescription("Get user details")
            .WithRepository(userRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestAndEntityFilter(request, userAccountFilter)
            .ExecuteAndMap(MapUserEntityToUser);

        var directAssignments = await strategyBuilderFactory.BuildGetListStrategy<ApplicationUserAccountHoldingAssignments>()
            .WithEntityDescription(EntityDescriptions.CphAssignment)
            .WithActionDescription("Get county parish holdings assigned to user")
            .WithRepository(cphAssignmentsRepository)
            .WithCancellationToken(cancellationToken)
            .WithEntityFilter(cphAssignmentsFilter)
            .ExecuteAndMap(MapCphAssignmentEntityToCphAssignment);

        var inboundDelegations = await strategyBuilderFactory.BuildGetListStrategy<CountyParishHoldingDelegations>()
            .WithEntityDescription(EntityDescriptions.CphDelegation)
            .WithActionDescription("Get all county parish holdings delegated to user")
            .WithRepository(cphDelegationsRepository)
            .WithCancellationToken(cancellationToken)
            .WithEntityFilter(inboundDelegationsFilter)
            .ExecuteAndMap(MapCphDelegationEntityToCphDelegation);

        var outboundDelegations = await strategyBuilderFactory.BuildGetListStrategy<CountyParishHoldingDelegations>()
            .WithEntityDescription(EntityDescriptions.CphDelegation)
            .WithActionDescription("Get all county parish holdings delegations associated with cphs owned by user")
            .WithRepository(cphDelegationsRepository)
            .WithCancellationToken(cancellationToken)
            .WithEntityFilter(outboundDelegationsFilter)
            .ExecuteAndMap(MapCphDelegationEntityToCphDelegation);

        return new UserProfile(userDetails, directAssignments, inboundDelegations, outboundDelegations);
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
            Active = DelegationsHelper.IsActiveDelegation(cphDelegationEntity),
        };
    }
}
