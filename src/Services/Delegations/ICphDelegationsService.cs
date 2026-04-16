// <copyright file="ICphDelegationsService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations;

using Defra.Identity.Models.Requests.Delegations.Commands;
using Defra.Identity.Models.Requests.Delegations.Queries;
using Defra.Identity.Models.Responses.Delegations;

public interface ICphDelegationsService
{
    Task<List<CphDelegation>> GetAll(GetCphDelegations request, CancellationToken cancellationToken = default);

    Task<CphDelegation> Get(GetCphDelegationById request, CancellationToken cancellationToken = default);

    Task<CphDelegation> Create(CreateCphDelegation request, CancellationToken cancellationToken = default);

    Task<CphDelegation> Update(UpdateCphDelegationById request, CancellationToken cancellationToken = default);

    Task Accept(AcceptCphDelegationById request, CancellationToken cancellationToken = default);

    Task Reject(RejectCphDelegationById request, CancellationToken cancellationToken = default);

    Task Revoke(RevokeCphDelegationById request, CancellationToken cancellationToken = default);

    Task Expire(ExpireCphDelegationById request, CancellationToken cancellationToken = default);

    Task<bool> Delete(DeleteCphDelegationById request, CancellationToken cancellationToken = default);
}
