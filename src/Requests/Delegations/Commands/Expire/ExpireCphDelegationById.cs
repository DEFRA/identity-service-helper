// <copyright file="ExpireCphDelegationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Commands.Expire;

using Defra.Identity.Requests.Common;

public class ExpireCphDelegationById : IOperationById
{
    public Guid Id { get; set; }
}
