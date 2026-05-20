// <copyright file="IUserService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users;

using Defra.Identity.Models.Requests.Users.Commands;
using Defra.Identity.Models.Requests.Users.Queries;
using Defra.Identity.Models.Responses.Users;

public interface IUserService
{
    Task<List<User>> GetAll(GetAllUsers request, CancellationToken cancellationToken = default);

    Task<User> Get(GetUserById request, CancellationToken cancellationToken = default);

    Task<User> Create(CreateUser request, CancellationToken cancellationToken = default);

    Task<User> Update(UpdateUserById request, CancellationToken cancellationToken = default);

    Task<User> Upsert(UpsertUserById request, CancellationToken cancellationToken = default);

    Task Delete(DeleteUserById request, CancellationToken cancellationToken = default);
}
