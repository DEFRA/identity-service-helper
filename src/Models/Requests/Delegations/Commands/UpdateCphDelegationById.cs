// <copyright file="UpdateCphDelegationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Delegations.Commands;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Common;

public class UpdateCphDelegationById : CphDelegationWriteOperation, IOperationById
{
    [Description(OpenApiMetadata.Id)]
    public Guid Id { get; set; }
}
