// <copyright file="ICphService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs;

using Defra.Identity.Requests.Cphs.Commands.Delete;
using Defra.Identity.Requests.Cphs.Commands.Expire;
using Defra.Identity.Requests.Cphs.Queries;
using Defra.Identity.Responses.Common;
using Defra.Identity.Responses.Cphs;

public interface ICphService
{
    Task<PagedResults<Cph>> GetAllPaged(GetCphs request, CancellationToken cancellationToken = default);

    Task<Cph> Get(GetCph request, CancellationToken cancellationToken = default);

    Task Expire(ExpireCph request, Guid operatorId, CancellationToken cancellationToken = default);

    Task Delete(DeleteCph request, Guid operatorId, CancellationToken cancellationToken = default);
}
