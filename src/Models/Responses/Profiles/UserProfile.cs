// <copyright file="UserProfile.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Profiles;

using Defra.Identity.Models.Responses.Assignments;
using Defra.Identity.Models.Responses.Delegations;
using Defra.Identity.Models.Responses.Users;

public class UserProfile
{
    public UserProfile(
        User userDetails,
        IEnumerable<CphAssignment> directAssignments,
        IEnumerable<CphDelegation> inboundDelegations,
        IEnumerable<CphDelegation> outboundDelegations)
    {
        UserDetails = userDetails;
        DirectAssignments = directAssignments;
        InboundDelegations = inboundDelegations;
        OutboundDelegations = outboundDelegations;
    }

    public User UserDetails { get; private set; }

    public IEnumerable<CphAssignment> DirectAssignments { get; private set; }

    public IEnumerable<CphDelegation> InboundDelegations { get; private set; }

    public IEnumerable<CphDelegation> OutboundDelegations { get; private set; }
}
