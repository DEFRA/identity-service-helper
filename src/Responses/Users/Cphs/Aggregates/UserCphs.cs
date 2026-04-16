// <copyright file="UserCphs.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Responses.Users.Cphs.Aggregates;

public class UserCphs
{
    public UserCphs(IEnumerable<UserAssociatedCph> associations, IEnumerable<UserDelegatedCph> delegations)
    {
        Associations = associations;
        Delegations = delegations;
    }

    public IEnumerable<UserAssociatedCph> Associations { get; }

    public IEnumerable<UserDelegatedCph> Delegations { get; }
}
