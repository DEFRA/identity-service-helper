// <copyright file="ProfileService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Profiles;

using Defra.Identity.Models.Requests.Profiles.Queries;
using Defra.Identity.Models.Responses.Profiles;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Assignments;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Services.Common;
using Defra.Identity.Services.Common.Builders.Strategy.Factories;
using Defra.Identity.Services.Common.Extensions;
using Defra.Identity.Services.Common.Filters;
using Defra.Identity.Services.Common.Mappers;
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
            FilterLibrary.Users.Active
                .AndAlso(userAccount => userAccount.Id == request.Id);

        var cphAssignmentsFilter =
            FilterLibrary.CphAssignments.Active
                .AndAlso(assignment => assignment.UserAccountId == request.Id);

        var inboundDelegationsFilter =
            FilterLibrary.CphDelegations.ActiveOrPending
                .AndAlso(delegation => delegation.DelegatedUserId == request.Id);

        var outboundDelegationsFilter =
            FilterLibrary.CphDelegations.ActiveOrPending
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
            .WithRequest(request)
            .WithEntityFilter(userAccountFilter)
            .ExecuteAndMap(UserMapper.MapUserEntityToUser);

        var directAssignments = await strategyBuilderFactory.BuildGetListStrategy<ApplicationUserAccountHoldingAssignments>()
            .WithEntityDescription(EntityDescriptions.CphAssignment)
            .WithActionDescription("Get county parish holdings assigned to user")
            .WithRepository(cphAssignmentsRepository)
            .WithCancellationToken(cancellationToken)
            .WithEntityFilter(cphAssignmentsFilter)
            .ExecuteAndMap(AssignmentMapper.MapCphAssignmentEntityToCphAssignment);

        var inboundDelegations = await strategyBuilderFactory.BuildGetListStrategy<CountyParishHoldingDelegations>()
            .WithEntityDescription(EntityDescriptions.CphDelegation)
            .WithActionDescription("Get all county parish holdings delegated to user")
            .WithRepository(cphDelegationsRepository)
            .WithCancellationToken(cancellationToken)
            .WithEntityFilter(inboundDelegationsFilter)
            .ExecuteAndMap(DelegationMapper.MapCphDelegationEntityToCphDelegation);

        var outboundDelegations = await strategyBuilderFactory.BuildGetListStrategy<CountyParishHoldingDelegations>()
            .WithEntityDescription(EntityDescriptions.CphDelegation)
            .WithActionDescription("Get all county parish holdings delegations associated with cphs owned by user")
            .WithRepository(cphDelegationsRepository)
            .WithCancellationToken(cancellationToken)
            .WithEntityFilter(outboundDelegationsFilter)
            .ExecuteAndMap(DelegationMapper.MapCphDelegationEntityToCphDelegation);

        return new UserProfile(userDetails, directAssignments, inboundDelegations, outboundDelegations);
    }
}
