// <copyright file="GetDelegations.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Queries;

public class GetDelegations
{
    public string? Status { get; set; } = "Active";
}
