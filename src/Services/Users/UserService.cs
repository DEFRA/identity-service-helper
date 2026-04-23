// <copyright file="UserService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users;

using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Users.Commands;
using Defra.Identity.Models.Requests.Users.Queries;
using Defra.Identity.Models.Responses.Assignments;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Models.Responses.Delegations;
using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Assignments;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Services.Common.Builders.Strategy.Factories;
using Defra.Identity.Services.Common.Extensions;
using Defra.Identity.Services.Common.Filters;
using Defra.Identity.Services.Common.Helpers;
using Defra.Identity.Services.Common.Selectors;
using Defra.Identity.Services.Users.Rules;
using Microsoft.Extensions.Logging;

public class UserService : IUserService
{
    private readonly IUsersRepository repository;
    private readonly ICphAssignmentsForAssigneeRepository cphAssignmentsForAssigneeRepository;
    private readonly ICphDelegationsForDelegateRepository cphDelegationsForDelegateRepository;
    private readonly ICphDelegatesForDelegatorRepository cphDelegatesForDelegatorRepository;
    private readonly ICphDelegationsForDelegatorRepository cphDelegationsForDelegatorRepository;
    private readonly IStrategyBuilderFactory<UserService> strategyBuilderFactory;
    private readonly ILogger<UserService> logger;

    public UserService(
        IUsersRepository repository,
        ICphAssignmentsForAssigneeRepository cphAssignmentsForAssigneeRepository,
        ICphDelegationsForDelegateRepository cphDelegationsForDelegateRepository,
        ICphDelegatesForDelegatorRepository cphDelegatesForDelegatorRepository,
        ICphDelegationsForDelegatorRepository cphDelegationsForDelegatorRepository,
        IStrategyBuilderFactory<UserService> strategyBuilderFactory,
        ILogger<UserService> logger)
    {
        this.repository = repository;
        this.cphAssignmentsForAssigneeRepository = cphAssignmentsForAssigneeRepository;
        this.cphDelegationsForDelegateRepository = cphDelegationsForDelegateRepository;
        this.cphDelegatesForDelegatorRepository = cphDelegatesForDelegatorRepository;
        this.cphDelegationsForDelegatorRepository = cphDelegationsForDelegatorRepository;
        this.strategyBuilderFactory = strategyBuilderFactory;
        this.logger = logger;

        this.cphDelegatesForDelegatorRepository
            .WithHoldingAssignmentsFilter(FiltersLibrary.CphAssignments.NotDeleted)
            .WithCountyParishHoldingsFilter(FiltersLibrary.Cphs.NotDeletedOrExpired)
            .WithDelegationsFilter(FiltersLibrary.CphDelegations.Aggregates.VisibleAndReferencesValid);

        this.cphDelegationsForDelegatorRepository
            .WithHoldingAssignmentsFilter(FiltersLibrary.CphAssignments.NotDeleted);

        this.strategyBuilderFactory
            .WithDefaultLogger(this.logger);
    }

    public async Task<List<User>> GetAll(GetAllUsers request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all animal users, includeHidden: {IncludeHidden}", request.IncludeInactive);
        Expression<Func<UserAccounts, bool>> filter = x => IncludeInactiveInferred(request) || x.DeletedBy == null;

        var userAccounts = await repository.GetList(filter, cancellationToken);

        var users = userAccounts.Select(entity => MapUserEntityToUser(entity)).ToList();

        return users;
    }

    public async Task<User> Get(GetUserById request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting user by id {Id}", request.Id);
        Expression<Func<UserAccounts, bool>> filter = x => x.Id == request.Id;

        var userAccount = await repository.GetSingle(filter, cancellationToken);

        if (userAccount == null)
        {
            logger.LogWarning("User with id {Id} not found", request.Id);
            throw new NotFoundException("user not found.");
        }

        var user = MapUserEntityToUser(userAccount);

        return user;
    }

    public async Task<User> Upsert(UpdateUser request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Upserting user with id {Id}", request.Id);
        var existingUser = await repository.GetSingle(x => x.Id.Equals(request.Id), cancellationToken);

        if (existingUser != null)
        {
            logger.LogInformation("User with id {Id} found, updating", request.Id);
            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.EmailAddress = request.Email;
            var updated = await repository.Update(existingUser, cancellationToken);

            return MapUserEntityToUser(updated);
        }

        logger.LogInformation("User with id {Id} not found, creating", request.Id);

        var userAccount = new UserAccounts()
        {
            Id = request.Id, EmailAddress = request.Email, FirstName = request.FirstName, LastName = request.LastName,
        };

        var result = await repository.Create(userAccount, cancellationToken);

        return MapUserEntityToUser(result);
    }

    public async Task<User> Update(UpdateUser request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating user with id {Id}", request.Id);
        var existingUser = await repository.GetSingle(x => x.Id.Equals(request.Id), cancellationToken);

        if (existingUser == null)
        {
            logger.LogWarning("User with id {Id} not found for update", request.Id);
            throw new NullReferenceException($"User with id {request.Id} not found.");
        }

        existingUser.FirstName = request.FirstName;
        existingUser.LastName = request.LastName;
        existingUser.EmailAddress = request.Email;
        existingUser.DisplayName = request.DisplayName;

        var updated = await repository.Update(existingUser, cancellationToken);

        return MapUserEntityToUser(updated);
    }

    public async Task<User> Create(CreateUser request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new user with email {Email}", request.Email);

        var newUser = new UserAccounts
        {
            EmailAddress = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DisplayName = request.DisplayName,
            CreatedById = request.OperatorId,
        };

        var createdUser = await repository.Create(newUser, cancellationToken);

        return MapUserEntityToUser(createdUser);
    }

    public async Task<bool> Delete(DeleteUser request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting user with id {Id} by operator {OperatorId}", request.Id, request.OperatorId);
        return await repository.Delete(x => x.Id == request.Id, request.OperatorId, cancellationToken);
    }

    public async Task<UserCphs> GetUserCphs(GetUserCphsByUserId request, CancellationToken cancellationToken = default)
    {
        var userAssociatedCphs = await strategyBuilderFactory.BuildGetAssociationsListStrategy<UserAccounts, ApplicationUserAccountHoldingAssignments>()
            .WithPrimaryEntityDescription("User account")
            .WithActionDescription("Get all county parish holdings associated with")
            .WithPrimaryRepository(repository)
            .WithAssociationsRepository(cphAssignmentsForAssigneeRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestAndPrimaryEntityFilter(request, userAccount => userAccount.Id == request.Id)
            .WithAssociatedEntityFilter(FiltersLibrary.CphAssignments.NotDeleted)
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
            .WithActionDescription("Get all county parish holding delegations associated with")
            .WithPrimaryRepository(repository)
            .WithAssociationsRepository(cphDelegationsForDelegateRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestAndPrimaryEntityFilter(request, userAccount => userAccount.Id == request.Id)
            .WithAssociatedEntityFilter(FiltersLibrary.CphDelegations.Aggregates.VisibleAndReferencesValid)
            .WithPrimaryEntityExistenceRules(rules => { rules.Add(RulesLibrary.Existence.NotSoftDeleted); })
            .ExecuteAndMap(MapCphDelegationEntityToCphDelegation);

        return new UserCphs(userAssociatedCphs, userDelegatedCphs);
    }

    public async Task<PagedResults<CphDelegate>> GetCphDelegatesForDelegator(GetCphDelegatesByDelegatorId request, CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildGetAssociationsPagedStrategy<UserAccounts, UserAccounts>()
            .WithPrimaryEntityDescription("User account")
            .WithActionDescription("Get user owned county parish holding unique delegation users associated with")
            .WithPrimaryRepository(repository)
            .WithAssociationsRepository(cphDelegatesForDelegatorRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestAndPrimaryEntityFilter(request, userAccount => userAccount.Id == request.Id)
            .WithAssociatedEntityFilter(FiltersLibrary.Users.NotDeleted)
            .WithPrimaryEntityExistenceRules(rules => { rules.Add(RulesLibrary.Existence.NotSoftDeleted); })
            .ExecuteAndMap(MapUserEntityToCphDelegate, SelectorLibrary.UserDisplayName);
    }

    public async Task<PagedResults<CphDelegation>> GetCphDelegationsForDelegateAssociatedWithDelegator(
        GetCphDelegationsByUserIdFiltered request,
        CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildGetAssociationsPagedStrategy<UserAccounts, CountyParishHoldingDelegations>()
            .WithPrimaryEntityDescription("User account")
            .WithActionDescription("Get county parish holding delegations for delegator filtered by")
            .WithPrimaryRepository(repository)
            .WithAssociationsRepository(cphDelegationsForDelegatorRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestAndPrimaryEntityFilter(request, userAccount => userAccount.Id == request.DelegatorId)
            .WithAssociatedEntityFilter(
                FiltersLibrary.CphDelegations.Aggregates.VisibleAndReferencesValid
                    .AndAlso(delegation => delegation.DelegatedUserId == request.Id))
            .WithPrimaryEntityExistenceRules(rules => { rules.Add(RulesLibrary.Existence.NotSoftDeleted); })
            .ExecuteAndMap(MapCphDelegationEntityToCphDelegation, SelectorLibrary.CphDelegationCphIdentifier);
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
            Active = DelegationHelper.IsActiveDelegation(cphDelegationEntity),
        };
    }

    private static CphDelegate MapUserEntityToCphDelegate(UserAccounts userEntity)
    {
        var delegatedUser = new CphDelegate();

        MapUserEntityToUser(userEntity, delegatedUser);

        return delegatedUser;
    }

    private static User MapUserEntityToUser(UserAccounts userEntity, User? mappingTarget = null)
    {
        var target = mappingTarget ?? new User();

        target.Id = userEntity.Id;
        target.Email = userEntity.EmailAddress;
        target.FirstName = userEntity.FirstName;
        target.LastName = userEntity.LastName;
        target.DisplayName = userEntity.DisplayName;

        return target;
    }

    private static bool IncludeInactiveInferred(GetAllUsers request)
    {
        return request.IncludeInactive != null &&
               (request.IncludeInactive == string.Empty ||
                request.IncludeInactive.Equals("true", StringComparison.InvariantCultureIgnoreCase));
    }
}
