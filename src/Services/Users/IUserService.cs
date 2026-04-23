// <copyright file="IUserService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users;

using Defra.Identity.Models.Requests.Users.Queries;
using Defra.Identity.Models.Responses.Users;

public interface IUserService
{
    Task<List<User>> GetAll(GetAllUsers request, CancellationToken cancellationToken = default);

    Task<User> Get(GetUserById request, CancellationToken cancellationToken = default);

    Task<User> Upsert(Models.Requests.Users.Commands.UpdateUser user, CancellationToken cancellationToken = default);

    Task<User> Update(Models.Requests.Users.Commands.UpdateUser user, CancellationToken cancellationToken = default);

    Task<User> Create(Models.Requests.Users.Commands.CreateUser user,  CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid id, Guid operatorId, CancellationToken cancellationToken = default);

    Task<bool> Validate(Guid id, string email, CancellationToken cancellationToken = default);
}
