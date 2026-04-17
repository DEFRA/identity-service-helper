// <copyright file="OperationByIdWithPagingFake.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Cphs.TestData;

using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Common.Queries;

public class OperationByIdWithPagingFake : PagedQuery, IOperationById
{
    public Guid Id { get; set; }
}
