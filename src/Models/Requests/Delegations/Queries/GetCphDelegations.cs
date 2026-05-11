// <copyright file="GetCphDelegations.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Delegations.Queries;

using System.ComponentModel;

public class GetCphDelegations
{
    [Description(OpenApiMetadata.IncludeInactive)]
    public string? IncludeInactive { get; set; } = null;
}
