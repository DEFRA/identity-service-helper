// <copyright file="GetCphUsersByCphId.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Cphs.Queries;

using Defra.Identity.Requests.Common;
using Defra.Identity.Requests.Common.Queries;

public class GetCphUsersByCphId : PagedQuery, IOperationById
{
    public Guid Id { get; set; }
}
