// <copyright file="GetCphs.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Cphs.Queries;

using Defra.Identity.Requests.Common.Queries;

public class GetCphs : PagedQueryBase
{
    public string? Expired { get; set; } = null;
}
