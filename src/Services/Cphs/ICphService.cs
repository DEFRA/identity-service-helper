// <copyright file="ICphService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs;

using Defra.Identity.Requests.Cphs.Commands;
using Defra.Identity.Requests.Cphs.Queries;
using Defra.Identity.Responses.Common;
using Defra.Identity.Responses.Cphs;

public interface ICphService
{
    Task<Guid> GetIdFromCphNumber(int county, int parish, int holding, CancellationToken cancellationToken = default);

    Task<PagedResults<Cph>> GetAllPaged(GetCphs request, CancellationToken cancellationToken = default);

    Task<Cph> Get(GetCphByCphId request, CancellationToken cancellationToken = default);

    Task Expire(ExpireCphByCphId request, Guid operatorId, CancellationToken cancellationToken = default);

    Task Delete(DeleteCphByCphId request, Guid operatorId, CancellationToken cancellationToken = default);

    Task<PagedResults<CphUser>> GetAllCphUsersPaged(GetCphUsersByCphId request, CancellationToken cancellationToken = default);
}
