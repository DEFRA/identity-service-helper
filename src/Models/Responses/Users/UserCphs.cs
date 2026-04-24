// <copyright file="UserCphs.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Users;

using Defra.Identity.Models.Responses.Assignments;
using Defra.Identity.Models.Responses.Delegations;

public class UserCphs
{
    public UserCphs(IEnumerable<CphAssignment> assignments, IEnumerable<CphDelegation> delegations)
    {
        Assignments = assignments;
        Delegations = delegations;
    }

    public IEnumerable<CphAssignment> Assignments { get; }

    public IEnumerable<CphDelegation> Delegations { get; }
}
