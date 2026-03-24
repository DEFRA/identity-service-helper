// <copyright file="ICphDelegationsService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations;

using Defra.Identity.Requests.Delegations.Commands.Create;
using Defra.Identity.Requests.Delegations.Commands.Update;
using Defra.Identity.Requests.Delegations.Queries;
using Defra.Identity.Responses.Delegations;

public interface ICphDelegationsService
{
    Task<List<CphDelegation>> GetAll(GetCphDelegations request, CancellationToken cancellationToken = default);

    Task<CphDelegation> Get(GetCphDelegationById request, CancellationToken cancellationToken = default);

    Task<CphDelegation> Update(UpdateCphDelegationById request, CancellationToken cancellationToken = default);

    Task<CphDelegation> Create(CreateCphDelegation request, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid id, Guid operatorId, CancellationToken cancellationToken = default);
}
