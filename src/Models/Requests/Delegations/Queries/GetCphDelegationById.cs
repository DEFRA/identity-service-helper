// <copyright file="GetCphDelegationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Delegations.Queries;

using System.ComponentModel;

public class GetCphDelegationById
{
    [Description(OpenApiMetadata.Id)]
    public Guid Id { get; set; }
}
