// <copyright file="PagedQuery.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Common.Queries;

public class PagedQuery
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public bool? OrderByDescending { get; set; } = false;
}
