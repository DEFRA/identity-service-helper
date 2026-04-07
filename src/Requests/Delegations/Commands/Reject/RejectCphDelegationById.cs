// <copyright file="RejectCphDelegationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Commands.Reject;

using Defra.Identity.Requests.Common;

public class RejectCphDelegationById : IOperationById
{
    public Guid Id { get; set; }
}
