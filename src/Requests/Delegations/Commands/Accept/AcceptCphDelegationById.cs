// <copyright file="AcceptCphDelegationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Commands.Accept;

using Defra.Identity.Requests.Common;

public class AcceptCphDelegationById : IOperationById
{
    public Guid Id { get; set; }
}
