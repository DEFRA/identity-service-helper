// <copyright file="PagedResults.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Common;

using System.ComponentModel;

public class PagedResults<T>
{
    public PagedResults(IEnumerable<T> items, int totalCount, int totalPages, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        TotalPages = totalPages;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    [Description(OpenApiMetadata.Paging.Items)]
    public IEnumerable<T> Items { get; }

    [Description(OpenApiMetadata.Paging.TotalCount)]
    public int TotalCount { get; }

    [Description(OpenApiMetadata.Paging.TotalPages)]
    public int TotalPages { get; }

    [Description(OpenApiMetadata.Paging.PageNumber)]
    public int PageNumber { get; }

    [Description(OpenApiMetadata.Paging.PageSize)]
    public int PageSize { get; }
}
