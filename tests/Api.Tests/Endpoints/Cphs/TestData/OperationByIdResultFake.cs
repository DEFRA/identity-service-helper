// <copyright file="OperationByIdResultFake.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Cphs.TestData;

public class OperationByIdResultFake
{
    public OperationByIdResultFake(Guid id, string description)
    {
        Id = id;
        Description = description;
    }

    public Guid Id { get; set; }

    public string Description { get; set; }
}
