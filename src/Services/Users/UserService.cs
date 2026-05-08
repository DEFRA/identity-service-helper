// <copyright file="UserService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users;

using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Users.Commands;
using Defra.Identity.Models.Requests.Users.Queries;
using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Users;
using Microsoft.Extensions.Logging;

public partial class UserService(
    IUsersRepository repository,
    ILogger<UserService> logger)
    : IUserService
{
    public async Task<List<User>> GetAll(GetAllUsers request, CancellationToken cancellationToken = default)
    {
        LogGettingAllUsersIncludeHidden(request.IncludeInactive);
        Expression<Func<UserAccounts, bool>> filter = x => IncludeInactiveInferred(request) || x.DeletedBy == null;

        var userAccounts = await repository.GetList(filter, cancellationToken);

        var users = userAccounts.Select(MapUserEntityToUser).ToList();

        return users;
    }

    public async Task<User> Get(GetUserById request, CancellationToken cancellationToken = default)
    {
        LogGettingUserById(request.Id);
        Expression<Func<UserAccounts, bool>> filter = x => x.Id == request.Id;

        var userAccount = await repository.GetSingle(filter, cancellationToken);

        if (userAccount == null)
        {
            LogUserWithIdNotFound(request.Id);
            throw new NotFoundException("user not found.");
        }

        var user = MapUserEntityToUser(userAccount);

        return user;
    }

    public async Task<User> Upsert(UpdateUser request, CancellationToken cancellationToken = default)
    {
        LogUpsertingUserWithId(request.Id);
        var existingUser = await repository.GetSingle(x => x.Id.Equals(request.Id), cancellationToken);

        if (existingUser != null)
        {
            LogUserWithIdFoundUpdating(request.Id);
            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.EmailAddress = request.Email;
            var updated = await repository.Update(existingUser, cancellationToken);

            return MapUserEntityToUser(updated);
        }

        LogUserWithIdNotFoundCreating(request.Id);

        var userAccount = new UserAccounts()
        {
            Id = request.Id, EmailAddress = request.Email, FirstName = request.FirstName, LastName = request.LastName,
        };

        var result = await repository.Create(userAccount, cancellationToken);

        return MapUserEntityToUser(result);
    }

    public async Task<User> Update(UpdateUser request, CancellationToken cancellationToken = default)
    {
        LogUpdatingUserWithId(request.Id);
        var existingUser = await repository.GetSingle(x => x.Id.Equals(request.Id), cancellationToken);

        if (existingUser == null)
        {
            LogUserWithIdNotFound(request.Id);
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
        LogCreatingNewUserWithEmail(request.Email);

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
        LogDeletingUserWithIdByOperatorId(request.Id, request.OperatorId);
        return await repository.Delete(x => x.Id == request.Id, request.OperatorId, cancellationToken);
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

    private static bool IncludeInactiveInferred(GetAllUsers request)
    {
        return request.IncludeInactive != null &&
               (request.IncludeInactive == string.Empty ||
                request.IncludeInactive.Equals("true", StringComparison.InvariantCultureIgnoreCase));
    }
}
