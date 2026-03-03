// <copyright file="GetCphUsers.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Cphs.Queries;

using Defra.Identity.Requests.Common.Queries;

public class GetCphUsers : PagedQuery
{
    public Guid Id { get; set; }
}
