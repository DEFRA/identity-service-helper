// <copyright file="OperationByIdWithPagingFake.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Common.TestData;

using System.Diagnostics.CodeAnalysis;
using Defra.Identity.Models.Requests.Common;
using Defra.Identity.Models.Requests.Common.Queries;

[ExcludeFromCodeCoverage]
public class OperationByIdWithPagingFake : PagedQuery, IOperationById<Guid>
{
    public Guid Id { get; set; }

    public string GetLoggableId()
    {
        return Id.ToString() ?? throw new InvalidOperationException("Id has not been set");
    }
}
