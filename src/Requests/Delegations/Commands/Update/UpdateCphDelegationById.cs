// <copyright file="UpdateCphDelegationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Commands.Update;

using Defra.Identity.Requests.Delegations.Commands.Common;

public class UpdateCphDelegationById : CphDelegationWriteOperation
{
    public required Guid Id { get; set; }
}
