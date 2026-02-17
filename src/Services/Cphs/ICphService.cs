// <copyright file="ICphService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs;

using Defra.Identity.Requests.Cphs.Queries;
using Defra.Identity.Responses.Common;
using Defra.Identity.Responses.Cphs;

public interface ICphService
{
    Task<PagedResults<Cph>> GetAllPaged(GetCphs request, CancellationToken cancellationToken = default);

    Task<Cph> Get(GetCphById request, CancellationToken cancellationToken = default);

    Task Delete(Guid id, Guid operatorId, CancellationToken cancellationToken = default);
}
