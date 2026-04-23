// <copyright file="GetCphDelegationsByUserIdFiltered.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Users.Queries;

using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Common.Queries;

public class GetCphDelegationsByUserIdFiltered : PagedQuery, IOperationById
{
    public Guid Id { get; set; }

    public Guid DelegatorId { get; set; }
}
