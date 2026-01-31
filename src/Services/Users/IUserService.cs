// <copyright file="IUserService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Requests.Users.Queries;
using Defra.Identity.Responses.Users;

public interface IUserService
{
    Task<List<User>> GetAll(GetUsers request, CancellationToken cancellationToken = default);

    Task<User> Get(GetUserById request, CancellationToken cancellationToken = default);

    Task<User> Upsert(Requests.Users.Commands.Update.UpdateUser user, CancellationToken cancellationToken = default);

    Task<User> Update(Requests.Users.Commands.Update.UpdateUser user, CancellationToken cancellationToken = default);
}
