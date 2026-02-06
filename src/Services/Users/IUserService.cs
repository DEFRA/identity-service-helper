// <copyright file="IUserService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users;

using Defra.Identity.Requests.Users.Queries;
using Defra.Identity.Responses.Users;

public interface IUserService
{
    Task<List<User>> GetAll(GetUsers request, CancellationToken cancellationToken = default);

    Task<User> Get(GetUserById request, CancellationToken cancellationToken = default);

    Task<User> Upsert(Requests.Users.Commands.Update.UpdateUser user, CancellationToken cancellationToken = default);

    Task<User> Update(Requests.Users.Commands.Update.UpdateUser user, CancellationToken cancellationToken = default);

    Task<User> Create(Requests.Users.Commands.Create.CreateUser user,  CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid id, CancellationToken cancellationToken = default);

    Task<bool> Activate(Guid id, CancellationToken cancellationToken = default);

    Task<bool> Suspend(Guid id, CancellationToken cancellationToken = default);
}
