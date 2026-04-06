// <copyright file="UpdateCphDelegationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Commands.Update;

using Defra.Identity.Requests.Common;

public class UpdateCphDelegationById : CphDelegationWriteOperation, IOperationById
{
    public Guid Id { get; set; }
}
