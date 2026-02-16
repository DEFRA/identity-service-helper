// <copyright file="IDelegationsService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Delegations;

using Defra.Identity.Requests.Delegations.Commands.Create;
using Defra.Identity.Requests.Delegations.Commands.Update;
using Defra.Identity.Requests.Delegations.Queries;
using Defra.Identity.Responses.Delegations;

public interface IDelegationsService
{
    Task<List<Delegation>> GetAll(GetDelegations request, CancellationToken cancellationToken = default);

    Task<Delegation> Get(GetDelegationById request, CancellationToken cancellationToken = default);

    Task<Delegation> Update(UpdateDelegation request, CancellationToken cancellationToken = default);

    Task<Delegation> Create(CreateDelegation request, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid id, Guid operatorId, CancellationToken cancellationToken = default);
}
