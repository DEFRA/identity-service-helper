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
    Task<List<CountyParishHoldingDelegation>> GetAll(GetDelegations request, CancellationToken cancellationToken = default);

    Task<CountyParishHoldingDelegation> Get(GetDelegationById request, CancellationToken cancellationToken = default);

    Task<CountyParishHoldingDelegation> Update(UpdateDelegation request, CancellationToken cancellationToken = default);

    Task<CountyParishHoldingDelegation> Create(CreateDelegation request, CancellationToken cancellationToken = default);

    Task<bool> Delete(Guid id, Guid operatorId, CancellationToken cancellationToken = default);
}
