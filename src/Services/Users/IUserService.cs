// <copyright file="IUserService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Users;

using Defra.Identity.Models.Requests.Users.Commands;
using Defra.Identity.Models.Requests.Users.Queries;
using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Responses.Users.Cphs.Aggregates;

public interface IUserService
{
    Task<List<User>> GetAll(GetAllUsers request, CancellationToken cancellationToken = default);

    Task<User> Get(GetUserById request, CancellationToken cancellationToken = default);

    Task<User> Upsert(UpdateUser request, CancellationToken cancellationToken = default);

    Task<User> Update(UpdateUser request, CancellationToken cancellationToken = default);

    Task<User> Create(CreateUser request,  CancellationToken cancellationToken = default);

    Task<bool> Delete(DeleteUser request, CancellationToken cancellationToken = default);

    Task<UserCphs> GetUserCphs(GetUserCphsByUserId request, CancellationToken cancellationToken = default);
}
