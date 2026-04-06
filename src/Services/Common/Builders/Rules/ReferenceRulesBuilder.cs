// <copyright file="ReferenceRulesBuilder.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>
namespace Defra.Identity.Services.Common.Builders.Rules;

using Defra.Identity.Repositories.Common.Composites;
using Defra.Identity.Services.Common.Builders.Rules.Models;

public class ReferenceRulesBuilder
{
    public List<ReferenceRule> ReferenceRules { get; } = [];

    public ReferenceRulesBuilder Add(IReference repository, Guid id, string description)
    {
        ReferenceRules.Add(new ReferenceRule(repository, id, description));

        return this;
    }
}
