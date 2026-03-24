// <copyright file="GetCphDelegations.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Delegations.Queries;

public class GetCphDelegations
{
    public string? Status { get; set; } = "Active";
}
