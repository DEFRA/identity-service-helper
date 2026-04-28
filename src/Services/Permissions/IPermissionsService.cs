// <copyright file="IPermissionsService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Permissions;

using Defra.Identity.Models.Requests.Permissions.Queries;
using Defra.Identity.Models.Requests.Users.Queries;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Models.Responses.Delegations;
using Defra.Identity.Models.Responses.Permissions;
using Defra.Identity.Models.Responses.Users;

public interface IPermissionsService
{
    Task<UserCphs> GetUserCphs(GetUserCphsByUserId request, CancellationToken cancellationToken = default);

    Task<PagedResults<User>> GetUserDelegates(GetUserDelegatesById request, CancellationToken cancellationToken = default);
}
