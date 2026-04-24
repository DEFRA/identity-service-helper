// <copyright file="ICphService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs;

using Defra.Identity.Models.Requests.Cphs.Commands;
using Defra.Identity.Models.Requests.Cphs.Common;
using Defra.Identity.Models.Requests.Cphs.Queries;
using Defra.Identity.Models.Responses.Assignments;
using Defra.Identity.Models.Responses.Common;
using Defra.Identity.Models.Responses.Cphs;

public interface ICphService
{
    Task<Guid> GetIdFromCphNumber(IOperationByCphNumber request, CancellationToken cancellationToken = default);

    Task<PagedResults<Cph>> GetAllPaged(GetCphs request, CancellationToken cancellationToken = default);

    Task<Cph> Get(GetCphByCphId request, CancellationToken cancellationToken = default);

    Task Expire(ExpireCphByCphId request, Guid operatorId, CancellationToken cancellationToken = default);

    Task Delete(DeleteCphByCphId request, Guid operatorId, CancellationToken cancellationToken = default);

    Task<PagedResults<CphAssignment>> GetCphAssignees(GetCphAssigneesByCphId request, CancellationToken cancellationToken = default);
}
