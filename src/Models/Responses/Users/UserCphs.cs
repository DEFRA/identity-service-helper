// <copyright file="UserCphs.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Users;

using Defra.Identity.Models.Responses.Assignments;
using Defra.Identity.Models.Responses.Delegations;

public class UserCphs
{
    public UserCphs(IEnumerable<CphAssignment> associations, IEnumerable<CphDelegation> delegations)
    {
        Associations = associations;
        Delegations = delegations;
    }

    public IEnumerable<CphAssignment> Associations { get; }

    public IEnumerable<CphDelegation> Delegations { get; }
}
