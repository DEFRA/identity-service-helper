// <copyright file="UserService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users;

using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Requests.Users.Commands.Create;
using Defra.Identity.Requests.Users.Commands.Update;
using Defra.Identity.Requests.Users.Queries;
using Defra.Identity.Responses.Users;
using Defra.Identity.Services;
using Defra.Identity.Services.Extensions;

public class UserService : IUserService
{

    private readonly IUsersRepository _repository;

    public UserService(IUsersRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<User>> GetAll(GetUsers request, CancellationToken cancellationToken = default)
    {
        Expression<Func<UserAccount, bool>> filter;
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            !string.Equals(request.Status, "Active", StringComparison.OrdinalIgnoreCase))
        {
            filter = x => x.Status.Name.ToLower() == request.Status.Trim().ToLower();
        }
        else
        {
            filter = x => x.Status.Name.ToLower() == "active";
        }

        var userAccounts = await _repository.GetList(filter, cancellationToken);

        var users = userAccounts.Select(userAccount => new User()
        {
            Id = userAccount.Id,
            Email = userAccount.EmailAddress,
            FirstName = userAccount.FirstName,
            LastName = userAccount.LastName,
            Status = userAccount.Status?.Name ?? string.Empty,
            DisplayName = userAccount.DisplayName,
        }).ToList();

        return users;
    }

    public async Task<User> Get(GetUserById request, CancellationToken cancellationToken = default)
    {
        Expression<Func<UserAccount, bool>> filter = x => x.Id == request.Id;

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            !string.Equals(request.Status, "Active", StringComparison.OrdinalIgnoreCase))
        {
            var requestedStatus = request.Status.Trim();

            filter = filter.AndAlso(x => x.Status.Name.ToLower() == requestedStatus.ToLower());
        }

        var userAccount = await _repository.GetSingle(filter, cancellationToken);

        if (userAccount == null)
        {
            return null;
        }

        var user = new User()
        {
            Id = userAccount.Id,
            Email = userAccount.EmailAddress,
            FirstName = userAccount.FirstName,
            LastName = userAccount.LastName,
            Status = userAccount.Status?.Name ?? string.Empty,
        };

        return user;
    }

    public async Task<User> Upsert(Requests.Users.Commands.Update.UpdateUser user, CancellationToken cancellationToken = default)
    {
        var existingUser = await _repository.GetSingle(x => x.Id.Equals(user.Id), cancellationToken);

        if (existingUser != null)
        {
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.EmailAddress = user.Email;
            var updated = await _repository.Update(existingUser, cancellationToken);

            return new User()
           {
               Id = updated.Id,
               Email = updated.EmailAddress,
               FirstName = updated.FirstName,
               LastName = updated.LastName,
           };
        }

        var userAccount = new UserAccount() { Id = user.Id, EmailAddress = user.Email, FirstName = user.FirstName, LastName = user.LastName };
        var result = await _repository.Create(userAccount, cancellationToken);
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
        var existingUser = await _repository.GetSingle(x => x.Id.Equals(user.Id), cancellationToken);

        if (existingUser == null)
        {
            throw new NullReferenceException($"User with id {user.Id} not found.");
        }

        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.EmailAddress = user.Email;
        existingUser.DisplayName = user.DisplayName;

        if (Guid.TryParse(user.OperatorId, out var operatorId))
        {
            existingUser.UpdatedBy = operatorId;
        }

        var updated = await _repository.Update(existingUser, cancellationToken);

        return new User
        {
            Id = updated.Id,
            Email = updated.EmailAddress,
            FirstName = updated.FirstName,
            LastName = updated.LastName,
            DisplayName = updated.DisplayName,
            Status = updated.Status?.Name ?? string.Empty,
        };
    }

    public async Task<User> Create(CreateUser user, CancellationToken cancellationToken = default)
    {
        var newUser = new UserAccount
        {
            EmailAddress = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DisplayName = user.DisplayName,
            CreatedBy = Guid.Parse(user.OperatorId),
        };

        var createdUser = await _repository.Create(newUser, cancellationToken);
        return new User()
        {
            Id = createdUser.Id,
            Email = createdUser.EmailAddress,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            DisplayName = createdUser.DisplayName,
            Status = createdUser.Status?.Name ?? string.Empty,
        };
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.Delete(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> Activate(Guid id, CancellationToken cancellationToken = default)
    {
      return await _repository.Activate(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> Suspend(Guid id, CancellationToken cancellationToken = default)
    {
       return await _repository.Suspend(x => x.Id == id, cancellationToken);
    }
}
