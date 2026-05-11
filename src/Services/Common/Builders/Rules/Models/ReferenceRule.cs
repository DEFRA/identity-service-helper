// <copyright file="ReferenceRule.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Builders.Rules.Models;

using Defra.Identity.Repositories.Common.Composites;

public class ReferenceRule
{
    public ReferenceRule(IReference repository, Guid id, string description)
    {
        Repository = repository;
        Id = id;
        Description = description;
    }

    public IReference Repository { get; }

    public Guid Id { get; }

    public string Description { get; }
}
