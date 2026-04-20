// <copyright file="UserService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users;

using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Users.Commands;
using Defra.Identity.Models.Requests.Users.Queries;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Models.Responses.Users.Delegates;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Repositories.Users.Cphs;
using Defra.Identity.Repositories.Users.Delegations;
using Defra.Identity.Responses.Users.Cphs;
using Defra.Identity.Responses.Users.Cphs.Aggregates;
using Defra.Identity.Services.Common.Builders.Strategy.Factories;
using Defra.Identity.Services.Common.Filters;
using Defra.Identity.Services.Common.Helpers;
using Defra.Identity.Services.Common.Selectors;
using Defra.Identity.Services.Users.Rules;
using Microsoft.Extensions.Logging;

public class UserService : IUserService
{
    private readonly IUsersRepository repository;
    private readonly IUserAssociatedCphsRepository userAssociatedCphsRepository;
    private readonly IUserDelegatedCphsRepository userDelegatedCphsRepository;
    private readonly IUserAssociatedDelegatesRepository userAssociatedDelegatesRepository;
    private readonly IStrategyBuilderFactory<UserService> strategyBuilderFactory;
    private readonly ILogger<UserService> logger;

    public UserService(
        IUsersRepository repository,
        IUserAssociatedCphsRepository userAssociatedCphsRepository,
        IUserDelegatedCphsRepository userDelegatedCphsRepository,
        IUserAssociatedDelegatesRepository userAssociatedDelegatesRepository,
        IStrategyBuilderFactory<UserService> strategyBuilderFactory,
        ILogger<UserService> logger)
    {
        this.repository = repository;
        this.userAssociatedCphsRepository = userAssociatedCphsRepository;
        this.userDelegatedCphsRepository = userDelegatedCphsRepository;
        this.userAssociatedDelegatesRepository = userAssociatedDelegatesRepository;
        this.strategyBuilderFactory = strategyBuilderFactory;
        this.logger = logger;

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
            .WithAssociationsRepository(userAssociatedCphsRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestAndPrimaryEntityFilter(request, userAccount => userAccount.Id == request.Id)
            .WithAssociatedEntityFilter(FiltersLibrary.HoldingAssignmentsNotDeletedFilter)
            .WithPrimaryEntityExistenceRules(rules => { rules.Add(RulesLibrary.Existence.NotSoftDeleted); })
            .ExecuteAndMap(
                entity => new UserAssociatedCph()
                {
                    AssociationId = entity.Id,
                    CountyParishHoldingId = entity.CountyParishHoldingId,
                    CountyParishHoldingNumber = entity.CountyParishHolding.Identifier,
                    ApplicationId = entity.ApplicationId,
                    RoleId = entity.RoleId,
                    RoleName = entity.Role.Name,
                });

        var userDelegatedCphs = await strategyBuilderFactory.BuildGetAssociationsListStrategy<UserAccounts, CountyParishHoldingDelegations>()
            .WithPrimaryEntityDescription("User account")
            .WithActionDescription("Get all county parish holding delegations associated with")
            .WithPrimaryRepository(repository)
            .WithAssociationsRepository(userDelegatedCphsRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestAndPrimaryEntityFilter(request, userAccount => userAccount.Id == request.Id)
            .WithAssociatedEntityFilter(FiltersLibrary.DelegationsNotDeletedOrExpiredFilter)
            .WithPrimaryEntityExistenceRules(rules => { rules.Add(RulesLibrary.Existence.NotSoftDeleted); })
            .ExecuteAndMap(
                entity => new UserDelegatedCph()
                {
                    DelegationId = entity.Id,
                    CountyParishHoldingId = entity.CountyParishHoldingId,
                    CountyParishHoldingNumber = entity.CountyParishHolding.Identifier,
                    DelegatingUserId = entity.DelegatingUserId,
                    DelegatingUserName = entity.DelegatingUser.DisplayName,
                    DelegatedUserRoleId = entity.DelegatedUserRoleId,
                    DelegatedUserRoleName = entity.DelegatedUserRole.Name,
                    InvitationExpiresAt = entity.InvitationExpiresAt,
                    InvitationAcceptedAt = entity.InvitationAcceptedAt,
                    InvitationRejectedAt = entity.InvitationRejectedAt,
                    RevokedAt = entity.RevokedAt,
                    ExpiresAt = entity.ExpiresAt,
                    RevokedById = entity.RevokedById,
                    RevokedByName = entity.RevokedByUser?.DisplayName,
                    Active = DelegationHelper.IsActiveDelegation(entity),
                });

        return new UserCphs(userAssociatedCphs, userDelegatedCphs);
    }

    public async Task<PagedResults<DelegatedUser>> GetUserOwnedCphDelegates(GetUserDelegatesByUserId request, CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildGetAssociationsPagedStrategy<UserAccounts, UserAccounts>()
            .WithPrimaryEntityDescription("User account")
            .WithActionDescription("Get user owned county parish holding unique delegation users associated with")
            .WithSetup(
                () =>
                {
                    this.userAssociatedDelegatesRepository
                        .WithHoldingAssignmentsFilter(FiltersLibrary.HoldingAssignmentsNotDeletedFilter)
                        .WithCountyParishHoldingsFilter(FiltersLibrary.CountyParishHoldingNotDeletedOrExpiredFilter)
                        .WithDelegationsFilter(FiltersLibrary.DelegationsNotDeletedOrExpiredFilter);
                })
            .WithPrimaryRepository(repository)
            .WithAssociationsRepository(userAssociatedDelegatesRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestAndPrimaryEntityFilter(request, userAccount => userAccount.Id == request.Id)
            .WithAssociatedEntityFilter(FiltersLibrary.UserAccountsNotDeletedFilter)
            .WithPrimaryEntityExistenceRules(rules => { rules.Add(RulesLibrary.Existence.NotSoftDeleted); })
            .ExecuteAndMap(MapUserEntityToDelegatedUser, SelectorLibrary.UserDisplayNameSelector);
    }

    private static DelegatedUser MapUserEntityToDelegatedUser(UserAccounts userEntity)
    {
        var delegatedUser = new DelegatedUser();

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
