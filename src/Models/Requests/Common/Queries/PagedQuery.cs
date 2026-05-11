// <copyright file="PagedQuery.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Common.Queries;

using System.ComponentModel;

public class PagedQuery
{
    [Description(OpenApiMetadata.Paging.PageNumber)]
    public int PageNumber { get; set; }

    [Description(OpenApiMetadata.Paging.PageSize)]
    public int PageSize { get; set; }

    [Description(OpenApiMetadata.Paging.OrderByProperty)]
    public string? OrderBy { get; set; } = string.Empty;

    [Description(OpenApiMetadata.Paging.OrderByDescending)]
    public bool? OrderByDescending { get; set; } = false;
}
