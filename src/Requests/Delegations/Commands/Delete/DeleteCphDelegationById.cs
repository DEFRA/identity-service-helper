// <copyright file="DeleteCphDelegationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Commands.Delete;

using Defra.Identity.Requests.Common;

public class DeleteCphDelegationById : IOperationById
{
    public Guid Id { get; set; }
}
