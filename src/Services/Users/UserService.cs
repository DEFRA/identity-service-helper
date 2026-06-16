// <copyright file="UserService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users;

using Defra.Identity.Models.Requests.Users.Commands;
using Defra.Identity.Models.Requests.Users.Queries;
using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Services.Common;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Common.Filters;
using Defra.Identity.Services.Common.Mappers;
using Defra.Identity.Services.Common.Strategy.Factories;
using Defra.Identity.Services.Users.Rules;
using FluentValidation;
using Microsoft.Extensions.Logging;

public class UserService : IUserService
{
    private readonly IUserRepository repository;
    private readonly IOperatorContext operatorContext;
    private readonly IStrategyBuilderFactory<UserService> strategyBuilderFactory;
    private readonly IValidator<CreateUser> createUserValidator;
    private readonly IValidator<UpdateUserById> updateUserValidator;
    private readonly IValidator<UpsertUserById> upsertUserValidator;

    public UserService(
        IUserRepository repository,
        IOperatorContext operatorContext,
        IStrategyBuilderFactory<UserService> strategyBuilderFactory,
        IValidator<CreateUser> createUserValidator,
        IValidator<UpdateUserById> updateUserValidator,
        IValidator<UpsertUserById> upsertUserValidator,
        ILogger<UserService> logger)
    {
        this.repository = repository;
        this.operatorContext = operatorContext;
        this.strategyBuilderFactory = strategyBuilderFactory;
        this.createUserValidator = createUserValidator;
        this.updateUserValidator = updateUserValidator;
        this.upsertUserValidator = upsertUserValidator;

        this.strategyBuilderFactory
            .WithDefaultLogger(logger)
            .WithDefaultOperatorContext(operatorContext)
            .WithDefaultEntityDescription(EntityDescriptions.User);
    }

    public async Task<List<User>> GetAll(GetAllUsers request, CancellationToken cancellationToken = default)
    {
        var includeInactiveInferred = IncludeInactiveInferred(request);
        var userFilter = includeInactiveInferred
            ? FilterLibrary.Users.All
            : FilterLibrary.Users.NotSoftDeleted;

        return await strategyBuilderFactory.BuildGetListStrategy<UserAccounts>()
            .WithActionDescription($"Get all users, includeHidden: {includeInactiveInferred}")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithEntityFilter(userFilter)
            .ExecuteAndMap(UserMapper.MapUserEntityToUser);
    }

    public async Task<User> Get(GetUserById request, CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildGetStrategy<UserAccounts>()
            .WithActionDescription("Get user")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(user => request.Id == user.Id)
            .ExecuteAndMap(UserMapper.MapUserEntityToUser);
    }

    public async Task<User> Create(CreateUser request, CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildCreateStrategy<UserAccounts>()
            .WithActionDescription("Create user")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequestValidation(() => createUserValidator.ValidateAsync(request, cancellationToken))
            .WithCreate(() => new UserAccounts()
            {
                EmailAddress = request.Email.ToLowerInvariant(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                CreatedById = operatorContext.OperatorId,
                CreatedAt = DateTime.UtcNow,
            })
            .ExecuteAndMap(UserMapper.MapUserEntityToUser);
    }

    public async Task<User> Update(UpdateUserById request, CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildUpdateStrategy<UserAccounts>()
            .WithActionDescription("Update user")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequestValidation(() => updateUserValidator.ValidateAsync(request, cancellationToken))
            .WithRequest(request)
            .WithEntityFilter(user => request.Id == user.Id)
            .WithExistenceRules(rules => rules.Add(RulesLibrary.Existence.NotSoftDeleted))
            .WithUpdate(user =>
            {
                user.EmailAddress = request.Email.ToLowerInvariant();
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.DisplayName = request.DisplayName;
            })
            .ExecuteAndMap(UserMapper.MapUserEntityToUser);
    }

    public async Task<User> Upsert(UpsertUserById request, CancellationToken cancellationToken = default)
    {
        return await strategyBuilderFactory.BuildUpsertStrategy<UserAccounts>()
            .WithActionDescription("Upsert user")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequestValidation(() => upsertUserValidator.ValidateAsync(request, cancellationToken))
            .WithRequest(request)
            .WithEntityFilter(user => request.Id.HasValue && request.Id.Value == user.Id)
            .WithExistenceRules(rules => rules.Add(RulesLibrary.Existence.NotSoftDeleted))
            .WithCreate(() => new UserAccounts
            {
                EmailAddress = request.Email.ToLowerInvariant(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                CreatedById = operatorContext.OperatorId,
                CreatedAt = DateTime.UtcNow,
            })
            .WithUpdate(user =>
            {
                user.EmailAddress = request.Email.ToLowerInvariant();
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.DisplayName = request.DisplayName;
            })
            .ExecuteAndMap(UserMapper.MapUserEntityToUser);
    }

    public async Task Delete(DeleteUserById request, CancellationToken cancellationToken = default)
    {
        await strategyBuilderFactory.BuildUpdateStrategy<UserAccounts>()
            .WithActionDescription("Delete user")
            .WithRepository(repository)
            .WithCancellationToken(cancellationToken)
            .WithRequest(request)
            .WithEntityFilter(user => request.Id == user.Id)
            .WithExistenceRules(rules => rules.Add(RulesLibrary.Existence.NotSoftDeleted))
            .WithUpdate(user =>
            {
                user.DeletedAt = DateTime.UtcNow;
                user.DeletedById = operatorContext.OperatorId;
            })
            .Execute();
    }

    private static bool IncludeInactiveInferred(GetAllUsers request)
    {
        return request.IncludeInactive != null &&
               (request.IncludeInactive == string.Empty ||
                request.IncludeInactive.Equals("true", StringComparison.InvariantCultureIgnoreCase));
    }
}
