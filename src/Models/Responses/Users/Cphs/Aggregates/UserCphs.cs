// <copyright file="UserCphs.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Users.Cphs.Aggregates;

using Defra.Identity.Models.Responses.Delegations;

public class UserCphs
{
    public UserCphs(IEnumerable<UserAssignedCph> associations, IEnumerable<CphDelegation> delegations)
    {
        Associations = associations;
        Delegations = delegations;
    }

    public IEnumerable<UserAssignedCph> Associations { get; }

    public IEnumerable<CphDelegation> Delegations { get; }
}
