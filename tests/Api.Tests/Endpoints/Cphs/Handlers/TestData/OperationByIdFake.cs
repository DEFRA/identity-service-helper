// <copyright file="OperationByIdFake.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Cphs.Handlers.TestData;

using Defra.Identity.Requests.Common;

public class OperationByIdFake : IOperationById
{
    public Guid Id { get; set; }
}
