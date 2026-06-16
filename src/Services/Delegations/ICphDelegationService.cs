// <copyright file="ICphDelegationService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations;

using Defra.Identity.Models.Requests.Delegations.Commands;
using Defra.Identity.Models.Requests.Delegations.Queries;
using Defra.Identity.Models.Responses.Delegations;

public interface ICphDelegationService
{
    Task<List<CphDelegation>> GetAll(CancellationToken cancellationToken = default);

    Task<CphDelegation> Get(GetCphDelegationById request, CancellationToken cancellationToken = default);

    Task<CphDelegation> Create(CreateCphDelegation request, CancellationToken cancellationToken = default);

    Task Accept(AcceptCphDelegationById request, CancellationToken cancellationToken = default);

    Task Reject(RejectCphDelegationById request, CancellationToken cancellationToken = default);

    Task Revoke(RevokeCphDelegationById request, CancellationToken cancellationToken = default);

    Task Expire(ExpireCphDelegationById request, CancellationToken cancellationToken = default);

    Task Delete(DeleteCphDelegationById request, CancellationToken cancellationToken = default);
}
