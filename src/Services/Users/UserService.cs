// <copyright file="UserService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Requests.Users.Commands.Create;
using Defra.Identity.Requests.Users.Commands.Update;
using Defra.Identity.Requests.Users.Queries;
using Defra.Identity.Responses.Users;
using Microsoft.Extensions.Logging;

public class UserService : IUserService
{
    private readonly IUsersRepository repository;
    private readonly ILogger<UserService> logger;

    public UserService(IUsersRepository repository, ILogger<UserService> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    public async Task<List<User>> GetAll(GetUsers request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all users");
        var userAccounts = await repository.GetList(x => true, cancellationToken);

        var users = userAccounts.Select(userAccount => new User()
        {
            Id = userAccount.Id,
            Email = userAccount.EmailAddress,
            FirstName = userAccount.FirstName,
            LastName = userAccount.LastName,
            DisplayName = userAccount.DisplayName,
        }).ToList();

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

        var user = new User()
        {
            Id = userAccount.Id,
            Email = userAccount.EmailAddress,
            FirstName = userAccount.FirstName,
            LastName = userAccount.LastName,
        };

        return user;
    }

    public async Task<User> Upsert(Requests.Users.Commands.Update.UpdateUser user, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Upserting user with id {Id}", user.Id);
        var existingUser = await repository.GetSingle(x => x.Id.Equals(user.Id), cancellationToken);

        if (existingUser != null)
        {
            logger.LogInformation("User with id {Id} found, updating", user.Id);
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.EmailAddress = user.Email;
            var updated = await repository.Update(existingUser, cancellationToken);

            return new User()
           {
               Id = updated.Id,
               Email = updated.EmailAddress,
               FirstName = updated.FirstName,
               LastName = updated.LastName,
           };
        }

        logger.LogInformation("User with id {Id} not found, creating", user.Id);
        var userAccount = new UserAccounts() { Id = user.Id, EmailAddress = user.Email, FirstName = user.FirstName, LastName = user.LastName };
        var result = await repository.Create(userAccount, cancellationToken);
        return new User()
        {
            Id = result.Id,
            Email = result.EmailAddress,
            FirstName = result.FirstName,
            LastName = result.LastName,
        };
    }

    public async Task<User> Update(UpdateUser user, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating user with id {Id}", user.Id);
        var existingUser = await repository.GetSingle(x => x.Id.Equals(user.Id), cancellationToken);

        if (existingUser == null)
        {
            logger.LogWarning("User with id {Id} not found for update", user.Id);
            throw new NullReferenceException($"User with id {user.Id} not found.");
        }

        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.EmailAddress = user.Email;
        existingUser.DisplayName = user.DisplayName;

        var updated = await repository.Update(existingUser, cancellationToken);

        return new User
        {
            Id = updated.Id,
            Email = updated.EmailAddress,
            FirstName = updated.FirstName,
            LastName = updated.LastName,
            DisplayName = updated.DisplayName,
        };
    }

    public async Task<User> Create(CreateUser user, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new user with email {Email}", user.Email);
        var newUser = new UserAccounts
        {
            EmailAddress = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DisplayName = user.DisplayName,
            CreatedById = user.OperatorId,
        };

        var createdUser = await repository.Create(newUser, cancellationToken);
        return new User()
        {
            Id = createdUser.Id,
            Email = createdUser.EmailAddress,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            DisplayName = createdUser.DisplayName,
        };
    }

    public async Task<bool> Delete(Guid id, Guid operatorId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting user with id {Id} by operator {OperatorId}", id, operatorId);
        return await repository.Delete(x => x.Id == id, operatorId, cancellationToken);
    }
}
