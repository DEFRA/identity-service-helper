// <copyright file="OperationByCphNumberWithPagingFake.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Cphs.TestData;

using Defra.Identity.Models.Requests.Common.Queries;
using Defra.Identity.Models.Requests.Cphs.Common;

public class OperationByCphNumberWithPagingFake : PagedQuery, IOperationByCphNumber
{
    public OperationByCphNumberWithPagingFake(int county, int parish, int holding, int pageNumber, int pageSize, bool? orderByDescending = null)
    {
        County = county;
        Parish = parish;
        Holding = holding;
        PageNumber = pageNumber;
        PageSize = pageSize;
        OrderByDescending = orderByDescending;
    }

    public int County { get; set; }

    public int Parish { get; set; }

    public int Holding { get; set; }
}
