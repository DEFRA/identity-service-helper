// <copyright file="IUserService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users;

using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Users.Commands.Create;
using Defra.Identity.Models.Requests.Users.Commands.Update;
using Defra.Identity.Models.Requests.Users.Queries;
using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Postgres.Database.Entities;

public interface IUserService
{
    Task<List<User>> GetAll(GetUsers request, CancellationToken cancellationToken = default);

    Task<User> Get(GetUserById request, CancellationToken cancellationToken = default);

    Task<User> Upsert(UpdateUser user, CancellationToken cancellationToken = default);

    Task<User> Update(UpdateUser user, CancellationToken cancellationToken = default);

    Task<User> Create(CreateUser user,  CancellationToken cancellationToken = default);
}
