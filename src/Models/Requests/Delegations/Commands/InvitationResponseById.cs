// <copyright file="InvitationResponseById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Delegations.Commands;

using Defra.Identity.Models.Requests.Common;

public abstract class InvitationResponseById : OperationById<Guid>
{
    public string InvitationToken { get; set; } = string.Empty;
}
