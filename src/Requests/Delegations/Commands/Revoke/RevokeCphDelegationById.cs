// <copyright file="RevokeCphDelegationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Commands.Revoke;

using Defra.Identity.Requests.Common;

public class RevokeCphDelegationById : IOperationById
{
    public Guid Id { get; set; }
}
